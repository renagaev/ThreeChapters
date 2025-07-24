<template>
  <StatsCard title="Прочитал(а)" :text="text" icon="bx:bible">

  </StatsCard>
</template>

<script setup lang="ts">
import {computed} from "vue";
import StatsCard from "@/components/stats/StatsCard.vue";

const props = defineProps<{ percentage: number }>()
const text = computed(() => Math.round(props.percentage * 1000) / 10 + "%")

</script>


<style scoped>

</style>
