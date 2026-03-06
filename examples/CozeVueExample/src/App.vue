<template>
  <el-config-provider :locale="zhCn">
    <div class="app-container">
      <el-tabs v-model="activeTab" class="main-tabs">
        <el-tab-pane label="聊天" name="chat">
          <HomeView />
        </el-tab-pane>
        <el-tab-pane label="工作流" name="workflow">
          <WorkflowView />
        </el-tab-pane>
      </el-tabs>
    </div>
  </el-config-provider>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import HomeView from '@/views/HomeView.vue'
import WorkflowView from '@/views/WorkflowView.vue'

const activeTab = ref('chat')
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html, body, #app {
  height: 100%;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* 全局 CSS 变量定义 */
:root {
  --color-primary: #409eff;
  --color-primary-light: #ecf5ff;
  --color-primary-dark: #337ecc;
  --color-background: #ffffff;
  --color-background-soft: #f5f7fa;
  --color-background-mute: #f0f2f5;
  --color-border: #dcdfe6;
  --color-heading: #303133;
  --color-text: #606266;
  --color-text-muted: #909399;
}

.app-container {
  height: 100%;
}

.main-tabs {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.main-tabs :deep(.el-tabs__header) {
  margin: 0;
  padding: 0 16px;
  background: #fff;
  border-bottom: 1px solid #e5e7eb;
}

.main-tabs :deep(.el-tabs__content) {
  flex: 1;
  overflow: hidden;
}

.main-tabs :deep(.el-tab-pane) {
  height: 100%;
}
</style>
