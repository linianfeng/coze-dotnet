<template>
  <div class="message-item" :class="[`message-${message.role}`]">
    <div class="message-avatar">
      <el-avatar :size="36" :class="message.role">
        {{ message.role === 'user' ? 'U' : 'AI' }}
      </el-avatar>
    </div>
    <div class="message-content">
      <div class="message-header">
        <span class="message-role">{{ message.role === 'user' ? '你' : 'AI 助手' }}</span>
        <span class="message-time">{{ formatTime(message.timestamp) }}</span>
      </div>
      <!-- 文件附件 -->
      <div v-if="message.files && message.files.length > 0" class="message-files">
        <div v-for="file in message.files" :key="file.id" class="file-item">
          <el-icon><Document /></el-icon>
          <span class="file-name">{{ file.name }}</span>
          <span class="file-size">{{ formatSize(file.size) }}</span>
        </div>
      </div>
      <!-- 消息内容 -->
      <div class="message-text" v-html="renderedContent"></div>
      <!-- 流式加载指示器 -->
      <div v-if="message.isStreaming" class="streaming-indicator">
        <span class="dot"></span>
        <span class="dot"></span>
        <span class="dot"></span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { marked } from 'marked'
import { Document } from '@element-plus/icons-vue'
import type { Message } from '@/types'

const props = defineProps<{
  message: Message
}>()

const renderedContent = computed(() => {
  if (!props.message.content) return ''
  // 使用 marked 渲染 Markdown
  return marked.parse(props.message.content) as string
})

function formatTime(date: Date): string {
  const d = new Date(date)
  return d.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

function formatSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}
</script>

<style scoped>
.message-item {
  display: flex;
  gap: 12px;
  padding: 16px;
  margin-bottom: 8px;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.message-user {
  background-color: #f0f9ff;
  flex-direction: row-reverse;
}

.message-assistant {
  background-color: #f9fafb;
}

.message-avatar {
  flex-shrink: 0;
}

.message-avatar .el-avatar {
  font-weight: 600;
}

.message-avatar .el-avatar.user {
  background-color: #3b82f6;
}

.message-avatar .el-avatar.assistant {
  background-color: #10b981;
}

.message-content {
  flex: 1;
  min-width: 0;
}

.message-user .message-content {
  text-align: right;
}

.message-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
}

.message-user .message-header {
  justify-content: flex-end;
}

.message-role {
  font-weight: 600;
  color: #374151;
  font-size: 14px;
}

.message-time {
  font-size: 12px;
  color: #9ca3af;
}

.message-files {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 8px;
}

.message-user .message-files {
  justify-content: flex-end;
}

.file-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 10px;
  background-color: #e5e7eb;
  border-radius: 6px;
  font-size: 13px;
}

.file-name {
  color: #374151;
}

.file-size {
  color: #9ca3af;
  font-size: 12px;
}

.message-text {
  text-align: left;
  line-height: 1.6;
  color: #1f2937;
  word-break: break-word;
}

.message-text :deep(p) {
  margin: 0 0 8px 0;
}

.message-text :deep(p:last-child) {
  margin-bottom: 0;
}

.message-text :deep(code) {
  background-color: #f3f4f6;
  padding: 2px 6px;
  border-radius: 4px;
  font-family: monospace;
  font-size: 0.9em;
}

.message-text :deep(pre) {
  background-color: #1f2937;
  color: #e5e7eb;
  padding: 12px;
  border-radius: 6px;
  overflow-x: auto;
  margin: 8px 0;
}

.message-text :deep(pre code) {
  background: none;
  padding: 0;
}

.streaming-indicator {
  display: flex;
  gap: 4px;
  padding: 8px 0;
}

.streaming-indicator .dot {
  width: 8px;
  height: 8px;
  background-color: #10b981;
  border-radius: 50%;
  animation: pulse 1.4s infinite ease-in-out both;
}

.streaming-indicator .dot:nth-child(1) {
  animation-delay: -0.32s;
}

.streaming-indicator .dot:nth-child(2) {
  animation-delay: -0.16s;
}

@keyframes pulse {
  0%, 80%, 100% {
    transform: scale(0.6);
    opacity: 0.5;
  }
  40% {
    transform: scale(1);
    opacity: 1;
  }
}
</style>
