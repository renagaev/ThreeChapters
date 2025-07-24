<script setup lang="ts">
import StatsCard from "@/components/stats/StatsCard.vue";
import {computed} from "vue";

const props = defineProps<{ times: number }>()

const text = computed(() => {
  // склоняй слово "раз"
  const times = props.times;
  const lastTwoDigits = times % 100;
  if (lastTwoDigits >= 11 && lastTwoDigits <= 19) {
    return `${times} раз`;
  }

  const lastDigit = times % 10;
  if (lastDigit === 1) {
    return `${times} раз`;
  }
  if (lastDigit >= 2 && lastDigit <= 4) {
    return `${times} раза`;
  }

  return `${times} раз`;
})

</script>

<template>
  <StatsCard title="Прочитал(а)" :text="text" icon="akar-icons:arrow-cycle"/>
</template>

<style scoped>

</style>
