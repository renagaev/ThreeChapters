<template>
  <Card class="max-w-xs w-full p-2 space-y-2 shadow border  rounded-md">
    <CardHeader class="p-0">
      <CardTitle class="text-base font-semibold max-w-full overflow-hidden text-ellipsis whitespace-nowrap"
                 :title="bookName">
        {{ bookName }}
      </CardTitle>
    </CardHeader>

    <CardContent class="p-0 space-y-2">
      <div class="text-sm text-gray-500">
        {{ readChapters }} / {{ totalChapters }}
      </div>
      <Progress
        :model-value="progressValue"
        class="h-2 rounded-full"
      />
    </CardContent>
  </Card>
</template>

<script setup>
import {computed} from 'vue'
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card"
import {Progress} from "@/components/ui/progress"

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
const isEmpty = computed(() => props.readChapters === 0)
const progressValue = computed(() => (props.readChapters / props.totalChapters) * 100)
</script>

<style scoped>
/* При необходимости можно добавить дополнительные стили */
</style>
