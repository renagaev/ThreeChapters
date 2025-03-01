<script setup lang="ts">
import {useStore} from "@/store";
import {computed, onMounted, ref} from "vue";
import CalendarHeatmap, {type Record} from "@/components/CalendarHeatmap.vue";

const props = defineProps<{ userId: number }>()

const store = useStore()
const data = ref(Array.of<Record>())
const selectedCell = ref<Record>()
onMounted(async () => {
  data.value = (await store.fetchUserReadChaptersByDay(props.userId)) as Record[]
  selectedCell.value = data.value.reduce((prev, current) => (prev && prev.date > current.date) ? prev : current)
})
const readText = computed(() => {
  if(!selectedCell.value){
    return ""
  }
  const formattedDate = selectedCell.value!.date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' })
  const count = selectedCell.value.count
  return `${formattedDate}: прочитано ${count} ${getChapterWord(count)}`;
});
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

function onCellClick(params) {
  const date = new Date(params.date);
  selectedCell.value = {date, count: params.count ?? 0}
}
</script>

<template>
  <div class="p-4">
    <p class="text-base text-gray-600 mb-1"> {{ readText }} </p>
    <calendar-heatmap :data="data" @clicked="onCellClick"/>
  </div>

</template>

<style scoped>

</style>
