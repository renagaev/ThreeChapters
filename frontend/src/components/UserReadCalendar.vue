<script setup lang="ts">
import {useStore} from "@/store";
import {onMounted, ref} from "vue";
import CalendarHeatmap, {type Record} from "@/components/CalendarHeatmap.vue";

const props = defineProps<{ userId: number }>()

const store = useStore()
const data = ref(Array.of<Record>())
onMounted(async () => {
  data.value = (await store.fetchUserReadChaptersByDay(props.userId)) as Record[]
})
</script>

<template>
  <calendar-heatmap :data="data"/>
</template>

<style scoped>

</style>
