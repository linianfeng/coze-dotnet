// API 类型定义

export interface ChatRequest {
  botId: string
  message: string
  userId?: string
}

export interface ChatWithFilesRequest {
  botId: string
  message: string
  fileIds?: string[]
  userId?: string
}

export interface ChatResponse {
  botId: string
  message: string
  response: string
}

export interface StreamChatEvent {
  eventType: 'message' | 'complete' | 'error'
  content?: string
  error?: string
}

export interface Bot {
  id: string
  name: string
  description?: string
  avatar?: string
}

export interface Session {
  id: string
  botId: string
  title: string
  messages: Message[]
  createdAt: Date
  updatedAt: Date
}

export interface Message {
  id: string
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  files?: FileInfo[]
  isStreaming?: boolean
}

export interface FileInfo {
  id: string
  name: string
  url: string
  size: number
  type?: string
}

export interface FileUploadResponse {
  id: string
  name: string
  url: string
  size: number
}

export interface ApiResponse<T> {
  success: boolean
  data?: T
  error?: string
}

// 工作流相关类型
export interface WorkflowRequest {
  workflowId: string
  parameters?: Record<string, unknown>
  userId?: string
}

export interface WorkflowResumeRequest {
  workflowRunId: string
  eventId: string
  resumeData?: unknown
}

export interface WorkflowResponse {
  executeId?: string
  output?: string
  debugUrl?: string
}

export interface StreamWorkflowEvent {
  eventType: 'message' | 'error' | 'done' | 'interrupt'
  content?: string
  error?: string
  executeId?: string
}

export interface WorkflowConfig {
  id: string
  name: string
  description?: string
  parameters?: WorkflowParameter[]
}

export interface WorkflowParameter {
  name: string
  type: 'string' | 'number' | 'boolean' | 'object'
  description?: string
  required?: boolean
  default?: unknown
}

export interface WorkflowSession {
  id: string
  workflowId: string
  status: 'running' | 'completed' | 'error' | 'interrupted'
  messages: WorkflowMessage[]
  executeId?: string
  createdAt: Date
  updatedAt: Date
}

export interface WorkflowMessage {
  id: string
  type: 'input' | 'output' | 'error'
  content: string
  timestamp: Date
}
