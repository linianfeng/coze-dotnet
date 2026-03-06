import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Session, Message } from '@/types'

function generateId(): string {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`
}

export const useSessionStore = defineStore('session', () => {
  // State
  const sessions = ref<Session[]>([])
  const currentSessionId = ref<string>('')

  // Getters
  const currentSession = computed(() =>
    sessions.value.find(s => s.id === currentSessionId.value)
  )

  const currentMessages = computed(() =>
    currentSession.value?.messages || []
  )

  const sortedSessions = computed(() =>
    [...sessions.value].sort((a, b) =>
      new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
    )
  )

  // Actions
  function createSession(botId: string, title = '新会话'): Session {
    const session: Session = {
      id: generateId(),
      botId,
      title,
      messages: [],
      createdAt: new Date(),
      updatedAt: new Date()
    }
    sessions.value.push(session)
    currentSessionId.value = session.id
    return session
  }

  function deleteSession(sessionId: string) {
    const index = sessions.value.findIndex(s => s.id === sessionId)
    if (index > -1) {
      sessions.value.splice(index, 1)
      // 如果删除的是当前会话，切换到最近的会话
      if (currentSessionId.value === sessionId) {
        currentSessionId.value = sortedSessions.value[0]?.id || ''
      }
    }
  }

  function switchSession(sessionId: string) {
    if (sessions.value.some(s => s.id === sessionId)) {
      currentSessionId.value = sessionId
    }
  }

  function addMessage(sessionId: string, message: Omit<Message, 'id' | 'timestamp'>): Message {
    const session = sessions.value.find(s => s.id === sessionId)
    if (!session) {
      throw new Error('Session not found')
    }

    const newMessage: Message = {
      ...message,
      id: generateId(),
      timestamp: new Date()
    }
    session.messages.push(newMessage)
    session.updatedAt = new Date()

    // 更新会话标题（使用第一条用户消息）
    if (session.title === '新会话' && message.role === 'user') {
      session.title = message.content.slice(0, 30) + (message.content.length > 30 ? '...' : '')
    }

    return newMessage
  }

  function updateMessage(sessionId: string, messageId: string, updates: Partial<Message>) {
    const session = sessions.value.find(s => s.id === sessionId)
    if (session) {
      const message = session.messages.find(m => m.id === messageId)
      if (message) {
        Object.assign(message, updates)
        session.updatedAt = new Date()
      }
    }
  }

  function clearMessages(sessionId: string) {
    const session = sessions.value.find(s => s.id === sessionId)
    if (session) {
      session.messages = []
      session.updatedAt = new Date()
    }
  }

  function getSessionsByBotId(botId: string): Session[] {
    return sessions.value.filter(s => s.botId === botId)
  }

  return {
    // State
    sessions,
    currentSessionId,
    // Getters
    currentSession,
    currentMessages,
    sortedSessions,
    // Actions
    createSession,
    deleteSession,
    switchSession,
    addMessage,
    updateMessage,
    clearMessages,
    getSessionsByBotId
  }
}, {
  persist: {
    key: 'coze-sessions',
    storage: localStorage,
    serializer: {
      serialize: (state) => JSON.stringify(state),
      deserialize: (value) => {
        const parsed = JSON.parse(value)
        // 转换日期字符串为 Date 对象
        if (parsed.sessions) {
          parsed.sessions = parsed.sessions.map((s: Session) => ({
            ...s,
            createdAt: new Date(s.createdAt),
            updatedAt: new Date(s.updatedAt),
            messages: s.messages.map((m: Message) => ({
              ...m,
              timestamp: new Date(m.timestamp)
            }))
          }))
        }
        return parsed
      }
    }
  }
})
