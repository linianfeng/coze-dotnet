<template>
  <div class="chat-panel">
    <!-- 消息列表 -->
    <div ref="messagesContainer" class="messages-container">
      <div v-if="messages.length === 0" class="empty-state">
        <el-empty description="开始新的对话吧">
          <template #image>
            <el-icon :size="64" color="#9ca3af"><ChatDotRound /></el-icon>
          </template>
        </el-empty>
      </div>
      <MessageItem
        v-for="message in messages"
        :key="message.id"
        :message="message"
      />
    </div>

    <!-- 输入区域 -->
    <div class="input-area">
      <!-- 错误提示 -->
      <el-alert
        v-if="error"
        :title="error"
        type="error"
        closable
        @close="clearError"
        class="error-alert"
      />

      <!-- 待发送文件 -->
      <div v-if="pendingFiles.length > 0" class="pending-files-preview">
        <span class="pending-label">附件:</span>
        <el-tag
          v-for="file in pendingFiles"
          :key="file.id"
          closable
          @close="removePendingFile(file.id)"
        >
          {{ file.name }}
        </el-tag>
      </div>

      <!-- 输入框 -->
      <div class="input-row">
        <FileUploader :max-size="10" />
        <el-input
          v-model="inputMessage"
          type="textarea"
          :rows="1"
          :autosize="{ minRows: 1, maxRows: 4 }"
          placeholder="输入消息..."
          :disabled="isLoading"
          @keydown.enter.exact.prevent="handleSend"
        />
        <el-button
          type="primary"
          :icon="isLoading ? '' : Promotion"
          :loading="isLoading"
          :disabled="!canSend"
          @click="handleSend"
        >
          {{ isLoading ? '' : '发送' }}
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { ChatDotRound, Promotion } from '@element-plus/icons-vue'
import { useChatStore, useSessionStore, useBotStore } from '@/stores'
import MessageItem from './MessageItem.vue'
import FileUploader from './FileUploader.vue'

const chatStore = useChatStore()
const sessionStore = useSessionStore()
const botStore = useBotStore()

const messagesContainer = ref<HTMLElement>()
const inputMessage = ref('')

const messages = computed(() => sessionStore.currentMessages)
const isLoading = computed(() => chatStore.isLoading)
const error = computed(() => chatStore.error)
const pendingFiles = computed(() => chatStore.pendingFiles)

const canSend = computed(() =>
  inputMessage.value.trim() && !isLoading.value && botStore.currentBotId
)

// 消息变化时自动滚动到底部
watch(messages, async () => {
  await nextTick()
  scrollToBottom()
}, { deep: true })

function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

async function handleSend() {
  const content = inputMessage.value.trim()
  if (!content || isLoading.value) return

  inputMessage.value = ''
  await chatStore.sendUserMessage(content, pendingFiles.value.length > 0 ? [...pendingFiles.value] : undefined)
}

function clearError() {
  chatStore.clearError()
}

function removePendingFile(fileId: string) {
  chatStore.removePendingFile(fileId)
}
</script>

<style scoped>
.chat-panel {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #fff;
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
}

.input-area {
  border-top: 1px solid #e5e7eb;
  padding: 16px;
  background-color: #f9fafb;
}

.error-alert {
  margin-bottom: 12px;
}

.pending-files-preview {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.pending-label {
  font-size: 13px;
  color: #6b7280;
}

.input-row {
  display: flex;
  gap: 8px;
  align-items: flex-end;
}

.input-row .el-textarea {
  flex: 1;
}
</style>
