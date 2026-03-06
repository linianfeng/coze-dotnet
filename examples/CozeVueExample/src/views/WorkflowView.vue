<template>
  <div class="workflow-view">
    <!-- Header -->
    <header class="header">
      <div class="header-left">
        <h1 class="logo">Coze Workflow</h1>
      </div>
      <div class="header-right">
        <span class="status" :class="connectionStatus">
          {{ connectionStatus === 'connected' ? '已连接' : '未连接' }}
        </span>
      </div>
    </header>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Sidebar -->
      <aside class="sidebar">
        <WorkflowSelector />
      </aside>

      <!-- Workflow Area -->
      <main class="workflow-area">
        <WorkflowPanel />
      </main>
    </div>

    <!-- Footer -->
    <footer class="footer">
      <div class="footer-left">
        <span>Coze 工作流示例</span>
      </div>
      <div class="footer-right">
        <span class="version">v1.0.0</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { checkHealth } from '@/api'
import WorkflowSelector from '@/components/workflow/WorkflowSelector.vue'
import WorkflowPanel from '@/components/workflow/WorkflowPanel.vue'

const connectionStatus = ref<'connected' | 'disconnected'>('disconnected')

onMounted(async () => {
  // 检查后端连接状态
  try {
    await checkHealth()
    connectionStatus.value = 'connected'
  } catch {
    connectionStatus.value = 'disconnected'
  }
})
</script>

<style scoped>
.workflow-view {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #fff;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  height: 60px;
  border-bottom: 1px solid #e5e7eb;
  background-color: #fff;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.logo {
  margin: 0;
  font-size: 20px;
  font-weight: 700;
  color: #1f2937;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.status {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #6b7280;
}

.status::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status.connected::before {
  background-color: #10b981;
}

.status.disconnected::before {
  background-color: #ef4444;
}

.main-content {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.sidebar {
  width: 260px;
  flex-shrink: 0;
  border-right: 1px solid #e5e7eb;
  overflow-y: auto;
}

.workflow-area {
  flex: 1;
  overflow-y: auto;
}

.footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  height: 40px;
  border-top: 1px solid #e5e7eb;
  background-color: #f9fafb;
  font-size: 12px;
  color: #6b7280;
}
</style>
