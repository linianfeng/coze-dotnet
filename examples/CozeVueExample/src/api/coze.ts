import type { ChatRequest, ChatWithFilesRequest, ChatResponse, FileUploadResponse, StreamChatEvent } from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || ''

/**
 * 发送聊天消息（非流式）
 */
export async function sendMessage(request: ChatRequest): Promise<ChatResponse> {
  const params = new URLSearchParams({
    botId: request.botId,
    message: request.message
  })
  if (request.userId) {
    params.append('userId', request.userId)
  }

  const response = await fetch(`${API_BASE}/coze/chat?${params.toString()}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json'
    }
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  return response.json()
}

/**
 * 发送聊天消息（流式响应）
 */
export async function* streamMessage(
  request: ChatWithFilesRequest
): AsyncGenerator<StreamChatEvent> {
  const params = new URLSearchParams({
    botId: request.botId,
    message: request.message
  })
  if (request.userId) {
    params.append('userId', request.userId)
  }
  if (request.fileIds && request.fileIds.length > 0) {
    params.append('fileIds', request.fileIds.join(','))
  }

  const response = await fetch(`${API_BASE}/coze/chat/stream?${params.toString()}`, {
    method: 'GET',
    headers: {
      Accept: 'text/event-stream'
    }
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  const reader = response.body?.getReader()
  if (!reader) {
    throw new Error('Response body is null')
  }

  const decoder = new TextDecoder()
  let buffer = ''

  try {
    while (true) {
      const { done, value } = await reader.read()
      if (done) break

      buffer += decoder.decode(value, { stream: true })
      const lines = buffer.split('\n')
      buffer = lines.pop() || ''

      for (const line of lines) {
        if (line.startsWith('data:')) {
          const data = line.slice(5).trim()
          if (data === '[DONE]') {
            yield { eventType: 'complete' }
            return
          }
          try {
            const parsed = JSON.parse(data)
            yield parsed
          } catch {
            // 如果不是 JSON，作为普通文本处理
            yield { eventType: 'message', content: data }
          }
        }
      }
    }
  } finally {
    reader.releaseLock()
  }

  yield { eventType: 'complete' }
}

/**
 * 上传文件
 */
export async function uploadFile(file: File): Promise<FileUploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch(`${API_BASE}/api/file/upload`, {
    method: 'POST',
    body: formData
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  return response.json()
}

/**
 * 检查健康状态
 */
export async function checkHealth(): Promise<{ status: string }> {
  const response = await fetch(`${API_BASE}/health`)
  if (!response.ok) {
    throw new Error(`Health check failed: ${response.status}`)
  }
  return response.json()
}
