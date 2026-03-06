<script setup lang="ts">
import type { WorkflowMessage } from '@/types'

defineProps<{
  message: WorkflowMessage
}>()

function formatTime(date: Date): string {
  return new Intl.DateTimeFormat('zh-CN', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  }).format(date)
}
</script>

<template>
  <div :class="['message-item', message.type]">
    <div class="message-header">
      <span class="message-type">
        {{ message.type === 'input' ? '输入' : message.type === 'output' ? '输出' : '错误' }}
      </span>
      <span class="message-time">{{ formatTime(message.timestamp) }}</span>
    </div>
    <div class="message-content">
      <pre v-if="message.type === 'input' || message.type === 'output'">{{ message.content }}</pre>
      <span v-else class="error-text">{{ message.content }}</span>
    </div>
  </div>
</template>

<style scoped>
.message-item {
  padding: 0.75rem;
  border-radius: 6px;
  border-left: 3px solid;
}

.message-item.input {
  background: var(--color-background);
  border-left-color: var(--color-primary);
}

.message-item.output {
  background: var(--color-background);
  border-left-color: #22c55e;
}

.message-item.error {
  background: #fef2f2;
  border-left-color: #ef4444;
}

.message-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.message-type {
  font-weight: 500;
  font-size: 0.875rem;
}

.message-item.input .message-type {
  color: var(--color-primary);
}

.message-item.output .message-type {
  color: #22c55e;
}

.message-item.error .message-type {
  color: #ef4444;
}

.message-time {
  font-size: 0.75rem;
  color: var(--color-text-muted);
}

.message-content {
  font-size: 0.875rem;
}

.message-content pre {
  margin: 0;
  white-space: pre-wrap;
  word-break: break-word;
  font-family: monospace;
  color: var(--color-text);
}

.error-text {
  color: #dc2626;
}
</style>
