<script setup lang="ts">
import { useWorkflowStore } from '@/stores/workflow'
import { computed } from 'vue'

const workflowStore = useWorkflowStore()

const workflows = computed(() => workflowStore.workflows)
const currentWorkflowId = computed(() => workflowStore.currentWorkflowId)

function selectWorkflow(workflowId: string) {
  workflowStore.setCurrentWorkflow(workflowId)
}
</script>

<template>
  <div class="workflow-selector">
    <h3 class="selector-title">选择工作流</h3>
    <div class="workflow-list">
      <button
        v-for="workflow in workflows"
        :key="workflow.id"
        :class="['workflow-item', { active: currentWorkflowId === workflow.id }]"
        @click="selectWorkflow(workflow.id)"
      >
        <div class="workflow-name">{{ workflow.name }}</div>
        <div v-if="workflow.description" class="workflow-description">
          {{ workflow.description }}
        </div>
      </button>
      <div v-if="workflows.length === 0" class="no-workflows">
        暂无可用工作流
      </div>
    </div>
  </div>
</template>

<style scoped>
.workflow-selector {
  padding: 1rem;
  background: var(--color-background-soft);
  border-radius: 8px;
}

.selector-title {
  margin: 0 0 1rem;
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-heading);
}

.workflow-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.workflow-item {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.75rem 1rem;
  background: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.workflow-item:hover {
  border-color: var(--color-primary);
  background: var(--color-background-mute);
}

.workflow-item.active {
  border-color: var(--color-primary);
  background: var(--color-primary-light);
}

.workflow-name {
  font-weight: 500;
  color: var(--color-heading);
}

.workflow-description {
  margin-top: 0.25rem;
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.no-workflows {
  padding: 1rem;
  text-align: center;
  color: var(--color-text-muted);
  font-style: italic;
}
</style>
