import type {
  WorkflowRequest,
  WorkflowResponse,
  StreamWorkflowEvent,
  WorkflowResumeRequest
} from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || ''

/**
 * 运行工作流（非流式）
 */
export async function runWorkflow(request: WorkflowRequest): Promise<WorkflowResponse> {
  const response = await fetch(`${API_BASE}/workflow/run`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  return response.json()
}

/**
 * 运行工作流（流式响应）
 */
export async function* streamWorkflow(
  request: WorkflowRequest
): AsyncGenerator<StreamWorkflowEvent> {
  const params = new URLSearchParams({
    workflowId: request.workflowId
  })
  if (request.parameters) {
    params.append('parameters', JSON.stringify(request.parameters))
  }
  if (request.userId) {
    params.append('userId', request.userId)
  }

  const response = await fetch(`${API_BASE}/workflow/run/stream?${params.toString()}`, {
    method: 'GET',
    headers: {
      Accept: 'text/event-stream'
    }
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  yield* parseSSEStream(response)
}

/**
 * 恢复工作流（流式响应）
 */
export async function* streamResumeWorkflow(
  request: WorkflowResumeRequest
): AsyncGenerator<StreamWorkflowEvent> {
  const response = await fetch(`${API_BASE}/workflow/resume/stream`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  })

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  yield* parseSSEStream(response)
}

/**
 * 获取工作流运行历史
 */
export async function getWorkflowHistory(
  workflowId: string,
  executeId: string
): Promise<unknown[]> {
  const response = await fetch(`${API_BASE}/workflow/history/${workflowId}/${executeId}`)

  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error: ${response.status}`)
  }

  return response.json()
}

/**
 * 解析 SSE 流
 */
async function* parseSSEStream(
  response: Response
): AsyncGenerator<StreamWorkflowEvent> {
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
            yield { eventType: 'done' }
            return
          }
          try {
            const parsed = JSON.parse(data) as StreamWorkflowEvent
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

  yield { eventType: 'done' }
}
