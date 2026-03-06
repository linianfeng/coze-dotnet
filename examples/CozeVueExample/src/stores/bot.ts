import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Bot } from '@/types'

export const useBotStore = defineStore('bot', () => {
  // State
  const bots = ref<Bot[]>([])
  const currentBotId = ref<string>('')

  // Getters
  const currentBot = computed(() =>
    bots.value.find(b => b.id === currentBotId.value)
  )

  const hasBots = computed(() => bots.value.length > 0)

  // Actions
  function addBot(bot: Bot) {
    const existing = bots.value.find(b => b.id === bot.id)
    if (existing) {
      // 更新现有 Bot
      Object.assign(existing, bot)
    } else {
      bots.value.push(bot)
    }
    // 如果是第一个 Bot，自动选中
    if (bots.value.length === 1) {
      currentBotId.value = bot.id
    }
  }

  function removeBot(botId: string) {
    const index = bots.value.findIndex(b => b.id === botId)
    if (index > -1) {
      bots.value.splice(index, 1)
      // 如果删除的是当前选中的 Bot，切换到第一个
      if (currentBotId.value === botId) {
        currentBotId.value = bots.value[0]?.id || ''
      }
    }
  }

  function setCurrentBot(botId: string) {
    if (bots.value.some(b => b.id === botId)) {
      currentBotId.value = botId
    }
  }

  function initDefaultBots() {
    if (bots.value.length === 0) {
      // 添加示例 Bot
      bots.value.push({
        id: '',
        name: '请配置 Bot ID',
        description: '点击右上角设置按钮配置您的 Bot ID'
      })
    }
  }

  return {
    // State
    bots,
    currentBotId,
    // Getters
    currentBot,
    hasBots,
    // Actions
    addBot,
    removeBot,
    setCurrentBot,
    initDefaultBots
  }
}, {
  persist: {
    key: 'coze-bots',
    storage: localStorage
  }
})
