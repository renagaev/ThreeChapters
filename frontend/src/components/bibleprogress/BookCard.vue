<template>
  <Dialog>
    <DialogTrigger :disabled="status != 'in-progress'">
      <Card :class="[  'w-full p-2 space-y-2 rounded-md transition-colors duration-300 text-left',
      statusClasses  ]">
        <CardHeader class="p-0">
          <CardTitle
            class="text-base font-semibold max-w-full overflow-hidden text-ellipsis whitespace-nowrap"
            :title="bookName">
            {{ bookName }}
          </CardTitle>
        </CardHeader>

        <CardContent class="p-0 space-y-2">
          <div
            class="text-sm"
            :class="statusTextClass"
          >
            {{ readChaptersCount }} / {{ totalChapters }}
          </div>
          <Progress
            :model-value="progressValue"
            class="h-2 rounded-full"
          />
        </CardContent>
      </Card>
    </DialogTrigger>
    <DialogScrollContent class="w-84 rounded-md">
      <DialogTitle>{{ bookName }}</DialogTitle>
      <div class="grid grid-cols-8 gap-1 justify-start w-fit">
        <div
          v-for="chapter in chapters"
          :key="chapter.number"
          class="w-8 h-8 rounded-md flex items-center justify-center"
          :class="chapter.read
            ? 'bg-primary text-white shadow-lg'
            : 'bg-gray-100 border border-gray-200 text-gray-400 shadow-inner'"
        >
          <p>{{ chapter.number }}</p>
        </div>
      </div>
    </DialogScrollContent>
  </Dialog>
</template>

<script setup lang="ts">
import {computed} from 'vue'
import {Card, CardContent, CardHeader, CardTitle} from '@/components/ui/card'
import {Progress} from '@/components/ui/progress'
import {Dialog, DialogScrollContent, DialogTitle, DialogTrigger} from "@/components/ui/dialog";

const props = defineProps({
  bookName: {
    type: String,
    required: true
  },
  totalChapters: {
    type: Number,
    required: true
  },
  readChapters: {
    type: Array<Number>,
    required: true
  }
})

const chapters = computed(() => Array.from({length: props.totalChapters}, (_, i) => ({
  number: i + 1,
  read: props.readChapters.includes(i + 1)
})))
const readChaptersCount = computed(() => props.readChapters.length)
const status = computed(() => {
  if (readChaptersCount.value === 0) {
    return 'not-started'
  } else if (readChaptersCount.value >= props.totalChapters) {
    return 'finished'
  } else {
    return 'in-progress'
  }
})

const progressValue = computed(() => {
  const ratio = readChaptersCount.value / props.totalChapters
  return ratio > 1 ? 100 : Math.round(ratio * 100)
})

const statusClasses = computed(() => {
  switch (status.value) {
    case 'not-started':
      return 'bg-gray-100 border border-gray-200 text-gray-400 shadow-inner'
    case 'finished':
      return 'bg-primary text-gray-100 border border-gray-800'
    default:
      return 'bg-white text-gray-800 border border-gray-200 shadow'
  }
})

// Дополнительные классы для текста (если нужно чуть тоньше контролировать)
const statusTextClass = computed(() => {
  if (status.value === 'not-started') {
    return 'text-gray-400'
  } else if (status.value === 'finished') {
    return 'text-gray-200'
  }
  return 'text-gray-500'
})
</script>

<style scoped>
/* Дополнительные стили при необходимости */
</style>
