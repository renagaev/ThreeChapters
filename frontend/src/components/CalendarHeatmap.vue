<script setup lang="ts">
// @ts-ignore
import CalHeatmap, {type Timestamp} from "cal-heatmap";
// @ts-ignore
import CalendarLabel from 'cal-heatmap/plugins/CalendarLabel';
import 'cal-heatmap/cal-heatmap.css';
import {computed, useTemplateRef, watch} from "vue";
import dayjs from 'dayjs'

export interface Record {
  date: Date,
  count: number
}

const props = defineProps<{
  data: Record[]
}>()

const emit = defineEmits<{
  (e: 'clicked', params: { date: number, count: number | null }): void
}>()
const colorClasses = ["bg-gray-400", "bg-gray-600", "bg-gray-800", "bg-primary"];
const stepsRef = useTemplateRef("colorSteps")
const colors = computed(() => Array.from(stepsRef.value!.children).map(el => getComputedStyle(el).backgroundColor))

function yyDaysTemplate(DateHelper: CalHeatmap.DateHelper, options: CalHeatmap.OptionsType) {
  return {
    name: 'yyDay',
    allowedDomainType: ['month'],
    rowsCount: () => 7,
    columnsCount: (ts: Timestamp) => {
      if (DateHelper.date(ts).startOf('month').valueOf() !== DateHelper.date().startOf('month').valueOf()) {
        return DateHelper.getWeeksCountInMonth(ts)
      } else {
        let firstBlockDate = DateHelper.getFirstWeekOfMonth(ts);
        let interval = DateHelper.intervals('day', firstBlockDate, DateHelper.date()).length;
        return Math.ceil((interval + 1) / 7);
      }
    },
    mapping: (startTimestamp: Timestamp, endTimestamp: Timestamp) => {
      const clampStart = DateHelper.getFirstWeekOfMonth(startTimestamp);
      const clampEnd = dayjs().startOf('day').add(8 - dayjs().day(), 'day')

      let x = -1;
      const pivotDay = clampStart.weekday();

      return DateHelper.intervals('day', clampStart, clampEnd).map((ts: Timestamp) => {
        const weekday = DateHelper.date(ts).weekday();
        if (weekday === pivotDay) {
          x += 1;
        }

        return {
          t: ts,
          x,
          y: weekday,
        };
      });
    },
    extractUnit: (ts: Timestamp) => {
      return DateHelper.date(ts).startOf('day').valueOf();
    },
  } as CalHeatmap.Template;
}

const cal: CalHeatmap = new CalHeatmap({});
cal.on("click", (event: PointerEvent, timestamp: number, value: number | null): void => emit("clicked", {
  date: timestamp,
  count: value
}))
watch(() => props.data.length, (v) => {
  if (v > 0) {
    paint()
  }
})

function paint() {
  const earliest = dayjs(Math.min(...props.data.map(x => x.date.getTime())))
  const latest = dayjs().endOf('month');
  const monthsCount = latest.diff(earliest.startOf('month'), 'month') + 1;

  cal.addTemplates(yyDaysTemplate)
  cal.paint({
    range: monthsCount,
    date: {
      locale: "ru",
      start: earliest.valueOf(),
      end: dayjs().endOf('day').valueOf()
    },
    data: {
      source: props.data,
      x: "date",
      y: "count",
      defaultValue: null
    },
    scale: {
      color: {
        range: colors.value,
        type: 'quantize',
        domain: [Math.min(...props.data.map(x => x.count)), Math.max(...props.data.map(x => x.count))],
      }
    },
    domain:
      {
        gutter: 3,
        type: "month",
        label: {text: 'MMM', textAlign: 'start', position: 'bottom'},
      }
    ,
    subDomain: {
      type: "yyDay",
      radius: 3,
      width: 16,
      height: 16,
      gutter: 3
    }
  },[[
    CalendarLabel, {
      textAlign: 'start',
      width: 20,
      text: () => ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"],
    }
  ]])
}


</script>

<template>
  <div ref="colorSteps">
    <div v-for="step in colorClasses" :class="[step]" :key="step"></div>
  </div>
  <div id="cal-heatmap" ref="calendar"/>
</template>

<style scoped>

</style>
