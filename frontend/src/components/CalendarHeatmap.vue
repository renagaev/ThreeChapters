<script setup lang="ts">
import CalHeatmap from "cal-heatmap";
import {computed, useTemplateRef} from "vue";
import 'cal-heatmap/cal-heatmap.css';

interface Record {
  date: string,
  count: number
}

const props = defineProps<{
  data: Record[]
}>()

const element = computed(() => useTemplateRef("calendar"))
const cal: CalHeatmap = new CalHeatmap({});
cal.paint({
  date: {start: new Date('2025-01-01')},
  data: {
    source: props.data, x: "date", y: "count"
  },
  scale: {
    color: {
      type: "linear",
    }
  },
  domain:
    {
      type: "month"
    }
  ,
  subDomain: {
    type: "day",
    radius: 3,
    width: 12,
    height: 12,
  }
})

</script>

<template>
  <div id="cal-heatmap" ref="calendar">

  </div>
</template>

<style scoped>

</style>
