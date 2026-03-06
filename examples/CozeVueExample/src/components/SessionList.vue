<template>
  <div class="session-list">
    <div class="session-header">
      <h3>会话历史</h3>
      <el-button type="primary" :icon="Plus" size="small" @click="createNewSession">
        新建
      </el-button>
    </div>
    <el-menu
      :default-active="currentSessionId"
      class="session-menu"
      @select="handleSelect"
    >
      <el-menu-item
        v-for="session in sortedSessions"
        :key="session.id"
        :index="session.id"
      >
        <div class="session-item">
          <div class="session-title">{{ session.title }}</div>
          <div class="session-meta">
            <span class="session-time">{{ formatDate(session.updatedAt) }}</span>
            <el-button
              :icon="Delete"
              size="small"
              text
              @click.stop="handleDelete(session.id)"
            />
          </div>
        </div>
      </el-menu-item>
    </el-menu>
    <el-empty v-if="sortedSessions.length === 0" description="暂无会话" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Plus, Delete } from '@element-plus/icons-vue'
import { useSessionStore, useBotStore } from '@/stores'
import { ElMessageBox } from 'element-plus'

const sessionStore = useSessionStore()
const botStore = useBotStore()

const currentSessionId = computed(() => sessionStore.currentSessionId)
const sortedSessions = computed(() => sessionStore.sortedSessions)

function createNewSession() {
  if (!botStore.currentBotId) {
    return
  }
  sessionStore.createSession(botStore.currentBotId)
}

function handleSelect(sessionId: string) {
  sessionStore.switchSession(sessionId)
}

async function handleDelete(sessionId: string) {
  try {
    await ElMessageBox.confirm('确定要删除这个会话吗？', '提示', {
      confirmButtonText: '删除',
      cancelButtonText: '取消',
      type: 'warning'
    })
    sessionStore.deleteSession(sessionId)
  } catch {
    // 用户取消
  }
}

function formatDate(date: Date): string {
  const d = new Date(date)
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))

  if (days === 0) {
    return d.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
  } else if (days === 1) {
    return '昨天'
  } else if (days < 7) {
    return `${days}天前`
  } else {
    return d.toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
  }
}
</script>

<style scoped>
.session-list {
  height: 100%;
  display: flex;
  flex-direction: column;
  background-color: #f9fafb;
  border-right: 1px solid #e5e7eb;
}

.session-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid #e5e7eb;
}

.session-header h3 {
  margin: 0;
  font-size: 16px;
  color: #374151;
}

.session-menu {
  border-right: none;
  flex: 1;
  overflow-y: auto;
}

.session-menu .el-menu-item {
  height: auto;
  padding: 12px 16px;
  line-height: 1.4;
}

.session-item {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.session-title {
  font-size: 14px;
  color: #1f2937;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.session-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.session-time {
  font-size: 12px;
  color: #9ca3af;
}
</style>
