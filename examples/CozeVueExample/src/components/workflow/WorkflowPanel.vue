<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useWorkflowStore } from '@/stores/workflow'
import WorkflowMessageItem from './WorkflowMessageItem.vue'

const workflowStore = useWorkflowStore()

const currentWorkflow = computed(() => workflowStore.currentWorkflow)
const currentSession = computed(() => workflowStore.currentSession)
const isLoading = computed(() => workflowStore.isLoading)
const error = computed(() => workflowStore.error)

// 参数表单 - 使用 Record<string, string> 简化类型，对于 object 类型存储 JSON 字符串
const paramValues = ref<Record<string, string>>({})

// 初始化参数默认值
function initDefaultParams() {
  if (currentWorkflow.value?.parameters) {
    for (const param of currentWorkflow.value.parameters) {
      if (param.default !== undefined) {
        // 将默认值转换为字符串
        if (typeof param.default === 'object') {
          paramValues.value[param.name] = JSON.stringify(param.default, null, 2)
        } else {
          paramValues.value[param.name] = String(param.default)
        }
      }
    }
  }
}

// 监听工作流变化，重置参数
watch(
  currentWorkflow,
  () => {
    paramValues.value = {}
    initDefaultParams()
  },
  { immediate: true }
)

async function handleRun() {
  // 构建参数
  const params: Record<string, unknown> = {}
  if (currentWorkflow.value?.parameters) {
    for (const param of currentWorkflow.value.parameters) {
      const value = paramValues.value[param.name]
      if (value !== undefined && value !== '') {
        // 根据参数类型转换值
        if (param.type === 'number') {
          params[param.name] = Number(value)
        } else if (param.type === 'boolean') {
          params[param.name] = value === 'true' || value === '1'
        } else if (param.type === 'object') {
          try {
            params[param.name] = JSON.parse(value)
          } catch {
            params[param.name] = value
          }
        } else {
          params[param.name] = value
        }
      } else if (param.required) {
        workflowStore.error = `参数 "${param.name}" 是必需的`
        return
      }
    }
  }

  await workflowStore.runWorkflowStream(Object.keys(params).length > 0 ? params : undefined)
}

function clearError() {
  workflowStore.clearError()
}
</script>

<template>
  <div class="workflow-panel">
    <!-- 参数输入区域 -->
    <div class="params-section">
      <h3 class="section-title">参数配置</h3>

      <div v-if="!currentWorkflow" class="no-workflow-selected">
        请先选择一个工作流
      </div>

      <div v-else-if="currentWorkflow.parameters?.length" class="params-form">
        <div v-for="param in currentWorkflow.parameters" :key="param.name" class="param-item">
          <label :for="param.name" class="param-label">
            {{ param.name }}
            <span v-if="param.required" class="required">*</span>
          </label>
          <p v-if="param.description" class="param-description">{{ param.description }}</p>

          <input
            v-if="param.type === 'string'"
            :id="param.name"
            v-model="paramValues[param.name]"
            type="text"
            class="param-input"
            :required="param.required"
          />
          <input
            v-else-if="param.type === 'number'"
            :id="param.name"
            v-model.number="paramValues[param.name]"
            type="number"
            class="param-input"
            :required="param.required"
          />
          <select
            v-else-if="param.type === 'boolean'"
            :id="param.name"
            v-model="paramValues[param.name]"
            class="param-select"
          >
            <option value="false">否</option>
            <option value="true">是</option>
          </select>
          <textarea
            v-else-if="param.type === 'object'"
            :id="param.name"
            v-model="paramValues[param.name]"
            class="param-textarea"
            :required="param.required"
            placeholder='{"key": "value"}'
          ></textarea>
          <input
            v-else
            :id="param.name"
            v-model="paramValues[param.name]"
            type="text"
            class="param-input"
            :required="param.required"
          />
        </div>
      </div>

      <div v-else class="no-params">
        该工作流无需配置参数
      </div>

      <!-- 运行按钮 -->
      <button
        v-if="currentWorkflow"
        class="run-button"
        :disabled="isLoading"
        @click="handleRun"
      >
        {{ isLoading ? '运行中...' : '运行工作流' }}
      </button>
    </div>

    <!-- 错误提示 -->
    <div v-if="error" class="error-banner">
      <span class="error-message">{{ error }}</span>
      <button class="error-close" @click="clearError">×</button>
    </div>

    <!-- 结果展示区域 -->
    <div class="results-section">
      <h3 class="section-title">执行结果</h3>

      <div v-if="!currentSession" class="no-results">
        运行工作流后，结果将显示在这里
      </div>

      <div v-else class="message-list">
        <WorkflowMessageItem
          v-for="message in currentSession.messages"
          :key="message.id"
          :message="message"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.workflow-panel {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  padding: 1rem;
}

.section-title {
  margin: 0 0 1rem;
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-heading);
}

.params-section {
  background: var(--color-background-soft);
  border-radius: 8px;
  padding: 1rem;
}

.no-workflow-selected,
.no-params,
.no-results {
  padding: 1rem;
  text-align: center;
  color: var(--color-text-muted);
  font-style: italic;
}

.params-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.param-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.param-label {
  font-weight: 500;
  color: var(--color-heading);
}

.required {
  color: #e74c3c;
  margin-left: 0.25rem;
}

.param-description {
  margin: 0;
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.param-input,
.param-select,
.param-textarea {
  padding: 0.5rem;
  border: 1px solid var(--color-border);
  border-radius: 4px;
  background: var(--color-background);
  color: var(--color-text);
  font-size: 0.875rem;
}

.param-input:focus,
.param-select:focus,
.param-textarea:focus {
  outline: none;
  border-color: var(--color-primary);
}

.param-textarea {
  min-height: 80px;
  resize: vertical;
  font-family: monospace;
}

.param-select {
  cursor: pointer;
}

.run-button {
  margin-top: 1rem;
  padding: 0.75rem 1.5rem;
  background: var(--color-primary);
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.run-button:hover:not(:disabled) {
  background: var(--color-primary-dark);
}

.run-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-banner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background: #fee2e2;
  border: 1px solid #ef4444;
  border-radius: 6px;
  color: #dc2626;
}

.error-message {
  flex: 1;
}

.error-close {
  padding: 0.25rem 0.5rem;
  background: transparent;
  border: none;
  font-size: 1.25rem;
  color: #dc2626;
  cursor: pointer;
}

.results-section {
  background: var(--color-background-soft);
  border-radius: 8px;
  padding: 1rem;
  min-height: 200px;
}

.message-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
</style>
