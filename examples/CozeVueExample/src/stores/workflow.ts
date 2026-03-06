import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  runWorkflow as runWorkflowApi,
  streamWorkflow,
  streamResumeWorkflow
} from '@/api/workflow'
import type {
  WorkflowConfig,
  WorkflowSession,
  WorkflowMessage
} from '@/types'

// 生成唯一 ID
function generateId(): string {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`
}

export const useWorkflowStore = defineStore('workflow', () => {
  // 预定义的工作流配置（可以从配置文件或 API 加载）
  const workflows = ref<WorkflowConfig[]>([
    {
      id: '7613227693571866660',
      name: '客户邮件PO提取',
      description: '',
      parameters: [
        {
          name: 'email_subject',
          type: 'string',
          description: ' 邮件主题',
          required: true
        },
         {
          name: 'email_body',
          type: 'string',
          description: '邮件正文内容',
          required: true
        },
        {
          name: 'sender_email',
          type: 'string',
          description: '发件人邮箱地址',
          required: true
        },
        {
          name: 'email_attachments',
          type: 'object',
          description: '邮件附件',
          required: false
        }
      ]
    }
  ])

  const currentWorkflowId = ref<string>('')
  const sessions = ref<Map<string, WorkflowSession>>(new Map())
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const currentWorkflow = computed(() => {
    return workflows.value.find(w => w.id === currentWorkflowId.value)
  })

  const currentSession = computed(() => {
    const sessionIds = Array.from(sessions.value.keys())
    const currentSession = sessionIds
      .map(id => sessions.value.get(id)!)
      .filter(s => s && s.workflowId === currentWorkflowId.value)
      .sort((a, b) => b.updatedAt.getTime() - a.updatedAt.getTime())[0]
    return currentSession || null
  })

  // Actions
  function setCurrentWorkflow(workflowId: string) {
    currentWorkflowId.value = workflowId
  }

  function createSession(workflowId: string): WorkflowSession {
    const session: WorkflowSession = {
      id: generateId(),
      workflowId,
      status: 'running',
      messages: [],
      createdAt: new Date(),
      updatedAt: new Date()
    }
    sessions.value.set(session.id, session)
    return session
  }

  function addMessage(
    sessionId: string,
    message: Omit<WorkflowMessage, 'id' | 'timestamp'>
  ): WorkflowMessage {
    const session = sessions.value.get(sessionId)
    if (!session) {
      throw new Error(`Session ${sessionId} not found`)
    }

    const newMessage: WorkflowMessage = {
      id: generateId(),
      ...message,
      timestamp: new Date()
    }
    session.messages.push(newMessage)
    session.updatedAt = new Date()
    return newMessage
  }

  function updateSession(
    sessionId: string,
    updates: Partial<WorkflowSession>
  ) {
    const session = sessions.value.get(sessionId)
    if (session) {
      Object.assign(session, updates, { updatedAt: new Date() })
    }
  }

  async function runWorkflow(parameters?: Record<string, unknown>) {
    if (!currentWorkflowId.value) {
      error.value = '请先选择一个工作流'
      return null
    }

    isLoading.value = true
    error.value = null

    // 创建会话
    const session = createSession(currentWorkflowId.value)

    // 添加输入消息
    if (parameters) {
      addMessage(session.id, {
        type: 'input',
        content: JSON.stringify(parameters, null, 2)
      })
    }

    try {
      // 调用同步 API
      const response = await runWorkflowApi({
        workflowId: currentWorkflowId.value,
        parameters
      })

      // 获取输出内容
      const outputContent = response.output || ''

      // 添加输出消息
      addMessage(session.id, {
        type: 'output',
        content: outputContent
      })

      // 更新会话状态
      updateSession(session.id, {
        status: 'completed',
        executeId: response.executeId
      })

      return { response: outputContent, executeId: response.executeId, debugUrl: response.debugUrl }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : '运行工作流失败'
      error.value = errorMessage

      updateSession(session.id, { status: 'error' })
      addMessage(session.id, {
        type: 'error',
        content: errorMessage
      })

      return null
    } finally {
      isLoading.value = false
    }
  }

  async function resumeWorkflow(
    workflowRunId: string,
    eventId: string,
    resumeData: unknown
  ) {
    isLoading.value = true
    error.value = null

    // 创建新会话
    const session = createSession(currentWorkflowId.value)
    session.status = 'running'

    // 添加恢复数据消息
    addMessage(session.id, {
      type: 'input',
      content: JSON.stringify(resumeData, null, 2)
    })

    // 创建输出消息占位
    const outputMessage = addMessage(session.id, {
      type: 'output',
      content: ''
    })

    let accumulatedContent = ''

    try {
      const stream = streamResumeWorkflow({
        workflowRunId,
        eventId,
        resumeData
      })

      for await (const event of stream) {
        // 先更新累积内容
        if (event.eventType === 'message' && event.content) {
          accumulatedContent += event.content
        }

        // 然后更新消息显示
        const message = session.messages.find(m => m.id === outputMessage.id)
        if (message) {
          switch (event.eventType) {
            case 'message':
              message.content = accumulatedContent
              break
            case 'error':
              message.content = accumulatedContent + `\n\n❌ 错误: ${event.error || '未知错误'}`
              updateSession(session.id, { status: 'error' })
              break
            case 'interrupt':
              updateSession(session.id, {
                status: 'interrupted',
                executeId: event.executeId
              })
              message.content = accumulatedContent + '\n\n⏸️ 工作流已暂停，等待恢复'
              break
          }
        }

        if (event.eventType === 'done' || event.eventType === 'error') {
          break
        }
      }

      updateSession(session.id, {
        status: 'completed',
        executeId: outputMessage.id
      })

      return { response: accumulatedContent }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : '恢复工作流失败'
      error.value = errorMessage

      updateSession(session.id, { status: 'error' })
      addMessage(session.id, {
        type: 'error',
        content: errorMessage
      })

      return null
    } finally {
      isLoading.value = false
    }
  }

  async function runWorkflowStream(parameters?: Record<string, unknown>) {
    if (!currentWorkflowId.value) {
      error.value = '请先选择一个工作流'
      return null
    }

    isLoading.value = true
    error.value = null

    const session = createSession(currentWorkflowId.value)

    if (parameters) {
      addMessage(session.id, {
        type: 'input',
        content: JSON.stringify(parameters, null, 2)
      })
    }

    const outputMessage = addMessage(session.id, {
      type: 'output',
      content: ''
    })

    let accumulatedContent = ''

    try {
      const stream = streamWorkflow({
        workflowId: currentWorkflowId.value,
        parameters
      })

      for await (const event of stream) {
        // 先更新累积内容
        if (event.eventType === 'message' && event.content) {
          accumulatedContent += event.content
        }

        // 然后更新消息显示
        const message = session.messages.find(m => m.id === outputMessage.id)
        if (message) {
          switch (event.eventType) {
            case 'message':
              message.content = accumulatedContent
              break
            case 'error':
              message.content = accumulatedContent + `\n\n❌ 错误: ${event.error || '未知错误'}`
              updateSession(session.id, { status: 'error' })
              break
            case 'interrupt':
              updateSession(session.id, {
                status: 'interrupted',
                executeId: event.executeId
              })
              message.content = accumulatedContent + '\n\n⏸️ 工作流已暂停，等待恢复'
              break
          }
        }

        if (event.eventType === 'done' || event.eventType === 'error') {
          break
        }
      }

      updateSession(session.id, {
        status: 'completed',
        executeId: outputMessage.id
      })

      return { response: accumulatedContent, executeId: outputMessage.id }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : '运行工作流失败'
      error.value = errorMessage

      updateSession(session.id, { status: 'error' })
      addMessage(session.id, {
        type: 'error',
        content: errorMessage
      })

      return null
    } finally {
      isLoading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  function clearSessions() {
    sessions.value.clear()
  }

  return {
    // State
    workflows,
    currentWorkflowId,
    sessions,
    isLoading,
    error,
    // Getters
    currentWorkflow,
    currentSession,
    // Actions
    setCurrentWorkflow,
    runWorkflow,
    runWorkflowStream,
    resumeWorkflow,
    clearError,
    clearSessions
  }
})
