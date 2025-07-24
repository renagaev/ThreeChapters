<script setup lang="ts">
import StatsCard from "@/components/stats/StatsCard.vue";
import {computed} from "vue";

const props = defineProps<{ days: number }>()

const text = computed(() => {
  // склоняй слово "дней"
  const times = props.days;
  const lastDigit = times % 10;
  const lastTwoDigits = times % 100;
  if (lastTwoDigits >= 11 && lastTwoDigits <= 19) {
    return `${times} дней`;
  }
  if (lastDigit === 1) {
    return `${times} день`;
  }
  if (lastDigit >= 2 && lastDigit <= 4) {
    return `${times} дня`;
  }
  return `${times} дней`;
})

</script>

<template>
  <StatsCard title="Серия" :text="text" icon="nimbus:fire"/>
</template>

<style scoped>

</style>
