<script setup lang="ts">
import StatsCard from "@/components/stats/StatsCard.vue";
import {computed} from "vue";

const props = defineProps<{ current: number }>()

const text = computed(() => Math.round(props.current * 1000) / 10 + "%");
const icon = computed(() => {
  const curr = props.current;
  if (curr < 0.2) {
    return 'mdi:network-strength-outline'
  }
  if (curr < 0.4) {
    return 'mdi:network-strength-1'
  }
  if (curr < 0.6) {
    return 'mdi:network-strength-2'
  }
  if (curr < 0.8) {
    return 'mdi:network-strength-3'
  }
  return 'mdi:network-strength-4'
})
</script>

<template>
  <StatsCard title="Сила привычки" :text="text" :icon="icon"/>
</template>

<style scoped>

</style>
