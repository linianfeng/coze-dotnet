<template>
  <div class="home-view">
    <!-- Header -->
    <header class="header">
      <div class="header-left">
        <h1 class="logo">Coze Chat</h1>
      </div>
      <div class="header-right">
        <BotSelector v-model="currentBotId" />
      </div>
    </header>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Sidebar -->
      <aside class="sidebar">
        <SessionList />
      </aside>

      <!-- Chat Area -->
      <main class="chat-area">
        <ChatPanel />
      </main>
    </div>

    <!-- Footer -->
    <footer class="footer">
      <div class="footer-left">
        <span class="status" :class="connectionStatus">
          {{ connectionStatus === 'connected' ? '已连接' : '未连接' }}
        </span>
      </div>
      <div class="footer-right">
        <span class="version">v1.0.0</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useBotStore } from '@/stores'
import { checkHealth } from '@/api'
import BotSelector from '@/components/BotSelector.vue'
import SessionList from '@/components/SessionList.vue'
import ChatPanel from '@/components/ChatPanel.vue'

const botStore = useBotStore()

const currentBotId = computed({
  get: () => botStore.currentBotId,
  set: (val) => botStore.setCurrentBot(val)
})

const connectionStatus = ref<'connected' | 'disconnected'>('disconnected')

onMounted(async () => {
  // 初始化默认 Bots
  botStore.initDefaultBots()

  // 检查后端连接状态
  try {
    await checkHealth()
    connectionStatus.value = 'connected'
  } catch {
    connectionStatus.value = 'disconnected'
  }
})
</script>

<style scoped>
.home-view {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #fff;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  height: 60px;
  border-bottom: 1px solid #e5e7eb;
  background-color: #fff;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.logo {
  margin: 0;
  font-size: 20px;
  font-weight: 700;
  color: #1f2937;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.main-content {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.sidebar {
  width: 260px;
  flex-shrink: 0;
}

.chat-area {
  flex: 1;
  overflow: hidden;
}

.footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  height: 40px;
  border-top: 1px solid #e5e7eb;
  background-color: #f9fafb;
  font-size: 12px;
  color: #6b7280;
}

.status {
  display: flex;
  align-items: center;
  gap: 6px;
}

.status::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status.connected::before {
  background-color: #10b981;
}

.status.disconnected::before {
  background-color: #ef4444;
}
</style>
