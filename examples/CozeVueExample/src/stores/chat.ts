import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useBotStore } from './bot'
import { useSessionStore } from './session'
import { streamMessage } from '@/api'
import type { FileInfo } from '@/types'

export const useChatStore = defineStore('chat', () => {
  // State
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const pendingFiles = ref<FileInfo[]>([])

  // Actions
  async function sendUserMessage(content: string, files?: FileInfo[]) {
    const botStore = useBotStore()
    const sessionStore = useSessionStore()

    if (!botStore.currentBot) {
      error.value = '请先选择一个 Bot'
      return null
    }

    // 确保有当前会话
    let session = sessionStore.currentSession
    if (!session || session.botId !== botStore.currentBotId) {
      session = sessionStore.createSession(botStore.currentBotId)
    }

    // 添加用户消息
    sessionStore.addMessage(session.id, {
      role: 'user',
      content,
      files: files || []
    })

    // 清空待发送文件
    pendingFiles.value = []

    // 创建助手消息占位
    const assistantMessage = sessionStore.addMessage(session.id, {
      role: 'assistant',
      content: '',
      isStreaming: true
    })

    isLoading.value = true
    error.value = null

    // 用于累积响应内容
    let accumulatedContent = ''

    try {
      // 使用流式 API
      const stream = streamMessage({
        botId: botStore.currentBotId,
        message: content,
        fileIds: files?.map(f => f.id)
      })

      for await (const event of stream) {
        if (event.eventType === 'message' && event.content) {
          // 累积内容并实时更新消息
          accumulatedContent += event.content
          sessionStore.updateMessage(session.id, assistantMessage.id, {
            content: accumulatedContent,
            isStreaming: true
          })
        } else if (event.eventType === 'complete') {
          // 流式传输完成
          sessionStore.updateMessage(session.id, assistantMessage.id, {
            content: accumulatedContent,
            isStreaming: false
          })
        } else if (event.eventType === 'error') {
          // 错误处理
          const errorMsg = event.error || '未知错误'
          error.value = errorMsg
          sessionStore.updateMessage(session.id, assistantMessage.id, {
            content: accumulatedContent + `\n\n❌ 错误: ${errorMsg}`,
            isStreaming: false
          })
        }
      }

      return { response: accumulatedContent }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : '发送消息失败'
      error.value = errorMessage
      // 更新助手消息为错误
      sessionStore.updateMessage(session.id, assistantMessage.id, {
        content: accumulatedContent ? `${accumulatedContent}\n\n❌ 错误: ${errorMessage}` : `❌ 错误: ${errorMessage}`,
        isStreaming: false
      })
      return null
    } finally {
      isLoading.value = false
    }
  }

  function addPendingFile(file: FileInfo) {
    pendingFiles.value.push(file)
  }

  function removePendingFile(fileId: string) {
    const index = pendingFiles.value.findIndex(f => f.id === fileId)
    if (index > -1) {
      pendingFiles.value.splice(index, 1)
    }
  }

  function clearPendingFiles() {
    pendingFiles.value = []
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    isLoading,
    error,
    pendingFiles,
    // Actions
    sendUserMessage,
    addPendingFile,
    removePendingFile,
    clearPendingFiles,
    clearError
  }
})
