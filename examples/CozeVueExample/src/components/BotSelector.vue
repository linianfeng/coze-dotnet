<template>
  <div class="bot-selector">
    <el-select
      v-model="localBotId"
      placeholder="选择 Bot"
      class="bot-select"
      @change="handleChange"
    >
      <el-option
        v-for="bot in bots"
        :key="bot.id"
        :label="bot.name"
        :value="bot.id"
      >
        <div class="bot-option">
          <span class="bot-name">{{ bot.name }}</span>
          <span v-if="bot.description" class="bot-desc">{{ bot.description }}</span>
        </div>
      </el-option>
    </el-select>
    <el-button type="primary" :icon="Plus" circle @click="showDialog = true" />
  </div>

  <!-- 添加 Bot 对话框 -->
  <el-dialog v-model="showDialog" title="添加 Bot" width="400px">
    <el-form :model="form" label-width="80px">
      <el-form-item label="Bot ID" required>
        <el-input v-model="form.id" placeholder="输入 Bot ID" />
      </el-form-item>
      <el-form-item label="名称" required>
        <el-input v-model="form.name" placeholder="输入 Bot 名称" />
      </el-form-item>
      <el-form-item label="描述">
        <el-input v-model="form.description" type="textarea" :rows="2" placeholder="可选描述" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="showDialog = false">取消</el-button>
      <el-button type="primary" @click="handleAdd" :disabled="!form.id || !form.name">
        添加
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import { useBotStore } from '@/stores'
import { ElMessage } from 'element-plus'

const botStore = useBotStore()

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const bots = computed(() => botStore.bots)
const localBotId = ref(props.modelValue)
const showDialog = ref(false)

const form = ref({
  id: '',
  name: '',
  description: ''
})

watch(() => props.modelValue, (val) => {
  localBotId.value = val
})

function handleChange(val: string) {
  emit('update:modelValue', val)
  botStore.setCurrentBot(val)
}

function handleAdd() {
  if (!form.value.id || !form.value.name) {
    ElMessage.warning('请填写 Bot ID 和名称')
    return
  }

  botStore.addBot({
    id: form.value.id,
    name: form.value.name,
    description: form.value.description || undefined
  })

  ElMessage.success('Bot 添加成功')
  showDialog.value = false
  form.value = { id: '', name: '', description: '' }

  // 选中新添加的 Bot
  emit('update:modelValue', botStore.bots[botStore.bots.length - 1].id)
}
</script>

<style scoped>
.bot-selector {
  display: flex;
  align-items: center;
  gap: 8px;
}

.bot-select {
  min-width: 200px;
}

.bot-option {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.bot-name {
  font-weight: 500;
}

.bot-desc {
  font-size: 12px;
  color: #9ca3af;
}
</style>
