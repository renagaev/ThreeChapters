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

function tooltipText(timestamp: number, value: number | null){
  const date = new Date(timestamp);
  const formattedDate = date.toLocaleDateString("ru-RU");
  const count = value !== null ? value : 0;

  function getChapterWord(count: number): string {
    const mod10 = count % 10;
    const mod100 = count % 100;

    if (mod100 >= 11 && mod100 <= 14) {
      return "глав";
    }
    if (mod10 === 1) {
      return "глава";
    }
    if (mod10 >= 2 && mod10 <= 4) {
      return "главы";
    }
    return "глав";
  }

  return `${formattedDate}: ${count} ${getChapterWord(count)}`;
}
</script>

<template>
  <calendar-heatmap :data="data" :tooltip-func="tooltipText"/>
</template>

<style scoped>

</style>
