<template>
  <el-upload
    ref="uploadRef"
    class="file-uploader"
    :auto-upload="false"
    :show-file-list="false"
    :on-change="handleFileChange"
    :accept="acceptTypes"
    multiple
  >
    <el-button :icon="Paperclip" circle />
  </el-upload>

  <!-- 待上传文件列表 -->
  <div v-if="pendingFiles.length > 0" class="pending-files">
    <div v-for="file in pendingFiles" :key="file.id" class="pending-file">
      <el-icon><Document /></el-icon>
      <span class="file-name">{{ file.name }}</span>
      <el-button
        :icon="Close"
        size="small"
        text
        @click="removeFile(file.id)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Paperclip, Document, Close } from '@element-plus/icons-vue'
import { useChatStore } from '@/stores'
import { uploadFile } from '@/api'
import { ElMessage } from 'element-plus'
import type { UploadFile } from 'element-plus'

const chatStore = useChatStore()

const props = defineProps<{
  maxSize?: number // MB
  accept?: string[]
}>()

const uploadRef = ref()
const isUploading = ref(false)

const acceptTypes = computed(() =>
  props.accept?.join(',') || '.pdf,.doc,.docx,.txt,.md,.jpg,.jpeg,.png,.gif'
)

const maxSizeBytes = computed(() =>
  (props.maxSize || 10) * 1024 * 1024
)

const pendingFiles = computed(() => chatStore.pendingFiles)

async function handleFileChange(uploadFileObj: UploadFile) {
  const file = uploadFileObj.raw
  if (!file) return

  // 检查文件大小
  if (file.size > maxSizeBytes.value) {
    ElMessage.error(`文件大小不能超过 ${props.maxSize || 10}MB`)
    return
  }

  isUploading.value = true
  try {
    const response = await uploadFile(file)
    chatStore.addPendingFile({
      id: response.id,
      name: response.name,
      url: response.url,
      size: response.size,
      type: file.type
    })
    ElMessage.success('文件上传成功')
  } catch (err) {
    const message = err instanceof Error ? err.message : '文件上传失败'
    ElMessage.error(message)
  } finally {
    isUploading.value = false
  }
}

function removeFile(fileId: string) {
  chatStore.removePendingFile(fileId)
}
</script>

<style scoped>
.file-uploader {
  display: inline-block;
}

.pending-files {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 8px;
  padding: 8px;
  background-color: #f3f4f6;
  border-radius: 8px;
}

.pending-file {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 10px;
  background-color: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  font-size: 13px;
}

.file-name {
  max-width: 150px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: #374151;
}
</style>
