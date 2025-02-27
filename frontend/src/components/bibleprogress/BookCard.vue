<template>
  <Card
    :class="[
      'max-w-xs w-full p-2 space-y-2 rounded-md transition-colors duration-300',
      statusClasses
    ]"
  >
    <CardHeader class="p-0">
      <CardTitle
        class="text-base font-semibold max-w-full overflow-hidden text-ellipsis whitespace-nowrap"
        :title="bookName"
      >
        {{ bookName }}
      </CardTitle>
    </CardHeader>

    <CardContent class="p-0 space-y-2">
      <div
        class="text-sm"
        :class="statusTextClass"
      >
        {{ readChapters }} / {{ totalChapters }}
      </div>
      <Progress
        :model-value="progressValue"
        class="h-2 rounded-full"
      />
    </CardContent>
  </Card>
</template>

<script setup lang="ts">
import {computed} from 'vue'
import {Card, CardContent, CardHeader, CardTitle} from '@/components/ui/card'
import {Progress} from '@/components/ui/progress'

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
    type: Number,
    required: true
  }
})

const status = computed(() => {
  if (props.readChapters === 0) {
    return 'not-started'
  } else if (props.readChapters >= props.totalChapters) {
    return 'finished'
  } else {
    return 'in-progress'
  }
})

const progressValue = computed(() => {
  const ratio = props.readChapters / props.totalChapters
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
/* При необходимости можно добавить дополнительные стили */
</style>
