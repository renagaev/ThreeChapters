<script setup lang="ts">
import * as d3 from 'd3';
import {ref, onMounted, watch, nextTick} from 'vue';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';

dayjs.locale('ru');

export interface Record {
  date: Date;
  count: number;
}

/* ——— Параметры визуала ——— */
const CELL = 16;          // размер ячейки
const GAP = 3;            // промежуток между ячейками
const COL_W = CELL + GAP; // ширина колонки‑недели
const RADIUS = 4;         // скругление углов
const MARGIN_TOP = 20;    // отступ сверху под названия месяцев
const LABEL_W = 16;       // ширина левой колонки с подписями дней недели

/* ——— Props / Emit ——— */
const props = defineProps<{ data: Record[] }>();
const emit = defineEmits<{ (e: 'clicked', p: { date: number; count: number | null }): void }>();

/* ——— Палитра (tailwind-цвета) ——— */
const colorClasses = ['bg-gray-400', 'bg-gray-600', 'bg-gray-800', 'bg-primary'];
const colorStepsRef = ref<HTMLDivElement>();
const emptyColor = 'rgba(229,231,235,1)';

function getColors() {
  return Array.from(colorStepsRef.value!.children).map(el => getComputedStyle(el as HTMLElement).backgroundColor);
}

/* ——— Refs ——— */
const gridSvgRef = ref<SVGSVGElement>();   // правая прокручиваемая SVG
const leftSvgRef = ref<SVGSVGElement>();   // фиксированная SVG слева (дни недели)
const scrollRef = ref<HTMLDivElement>();   // контейнер‑скроллер
const fadeLeft = ref(false);
const fadeRight = ref(false);

/* ——— Подсобная функция: вычисляем, нужны ли фейды ——— */
function updateFades() {
  const el = scrollRef.value;
  if (!el) return;
  fadeLeft.value = el.scrollLeft > 0;
  fadeRight.value = el.scrollLeft < el.scrollWidth - el.clientWidth - 1;
}

/* ——— Основная функция рендера ——— */
function render() {
  if (!gridSvgRef.value || !leftSvgRef.value) return;

  // 1) map<деньUTC, count>
  const map = new Map<number, number>();
  props.data.forEach(r => map.set(dayjs(r.date).startOf('day').valueOf(), r.count));

  // 2) диапазон дат
  const minTs = Math.min(...map.keys());
  const firstWeekStart = dayjs(minTs).startOf('week');
  const today = dayjs().startOf('day');

  type Cell = { ts: number; count: number | null; x: number; y: number; weekIdx: number };
  const days: Cell[] = [];
  for (let d = firstWeekStart, i = 0; d.isBefore(today) || d.isSame(today); d = d.add(1, 'day'), i++) {
    const weekIdx = Math.floor(i / 7);
    const row = i % 7;
    days.push({
      ts: d.valueOf(),
      count: map.get(d.valueOf()) ?? null,
      x: weekIdx * COL_W,
      y: MARGIN_TOP + row * COL_W,
      weekIdx
    });
  }
  const weeksCount = d3.max(days, d => d.weekIdx)! + 1;

  // 3) размеры SVG
  const gridW = weeksCount * COL_W - GAP; // последняя колонка без лишнего GAP
  const gridH = MARGIN_TOP + 7 * COL_W - GAP;

  /* 1) собираем палитру */
  const palette = getColors();      // ['rgb(…)', …] длиной 4-5
  const emptyColor = 'rgba(229,231,235,1)';

  /* 2) максимальное значение в данных */
  const max = d3.max(props.data, d => d.count) || 1;
  const min = d3.min(props.data, d => d.count) || 0;

  /* 3) шкала symlog: отдаёт число 0…(palette.length-1) */
  const symlog = d3.scaleSymlog()          // лог+линейная смесь
    .domain([min, max])                      // 0 → 0, max → palette.length-1
    .range([0, palette.length - 1])
    .clamp(true);                          // за пределы не выходит

  /* 4) функция цвета */
  const colorScale = (v: number | null) =>
    v == null ? emptyColor
      : palette[Math.round(symlog(v))];

  /* -------- SVG Сетка ---------- */
  const gridSvg = d3.select(gridSvgRef.value)
    .attr('width', gridW)
    .attr('height', gridH)
    .attr('viewBox', `0 0 ${gridW} ${gridH + 1}`)
    .style('font-family', 'sans-serif')
    .style('font-size', '9px');

  gridSvg.selectAll('*').remove();
  const g = gridSvg.append('g');

  // ячейки
  let selected: d3.Selection<SVGRectElement, Cell, any, any> | null = null;

  function setStroke(sel: d3.Selection<SVGRectElement, Cell, any, any>) {
    if (selected) selected.attr('stroke', null).attr('stroke-width', null);
    selected = sel.attr('stroke', '#000').attr('stroke-width', 2);
  }

  g.selectAll('rect.cell')
    .data(days)
    .enter()
    .append('rect')
    .attr('class', 'cell')
    .attr('x', d => d.x)
    .attr('y', d => d.y)
    .attr('width', CELL)
    .attr('height', CELL)
    .attr('rx', RADIUS)
    .attr('ry', RADIUS)
    .attr('fill', d => (d.count === null ? emptyColor : colorScale(d.count)))
    .attr('tabindex', 0)
    .on('mousedown', e => e.preventDefault())
    .on('mouseover', function () {
      setStroke(d3.select(this));
    })
    .on('focus', function () {
      setStroke(d3.select(this));
    })
    .on('click', function (_, d) {
      setStroke(d3.select(this));
      emit('clicked', {date: d.ts, count: d.count});
      (this as SVGRectElement).blur();
    });

  // подписи месяцев (первый столбец каждого месяца)
  type Label = { x: number; text: string; w: number };
  const labels: Label[] = [];
  const seenMonths = new Set<number>();
  days.forEach(d => {
    const month = dayjs(d.ts).month();
    if (!seenMonths.has(month)) {
      labels.push({x: d.x + 2, text: dayjs(d.ts).format('MMM'), w: 0});
      seenMonths.add(month);
    }
  });
  // замер и фильтр перекрытий
  const meas = g.append('g');
  meas.selectAll('text').data(labels).enter().append('text').text(d => d.text).each(function (d) {
    d.w = (this as SVGTextElement).getBBox().width;
  });
  meas.remove();
  labels.sort((a, b) => a.x - b.x);
  const finalLabels = labels.filter((l, i) => i === 0 || l.x > labels[i - 1].x + labels[i - 1].w + 4);

  g.selectAll('text.month')
    .data(finalLabels)
    .join('text')
    .attr('class', 'month')
    .attr('x', d => d.x)
    .attr('y', MARGIN_TOP / 2)
    .text(d => d.text);

  /* -------- SVG слева (дни недели) ---------- */
  const weekDays = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
  const leftSvg = d3.select(leftSvgRef.value)
    .attr('width', LABEL_W)
    .attr('height', gridH)
    .attr('viewBox', `0 0 ${LABEL_W} ${gridH}`)
    .style('font-family', 'sans-serif')
    .style('font-size', '9px');

  leftSvg.selectAll('*').remove();
  leftSvg.selectAll('text')
    .data(weekDays)
    .enter()
    .append('text')
    .attr('x', LABEL_W / 2)
    .attr('y', (_, i) => MARGIN_TOP + i * COL_W + CELL / 2 + 4)
    .attr('text-anchor', 'middle')
    .text(d => d);
}

/* ——— Lifecycle ——— */
onMounted(async () => {
  render();
  await nextTick();
  if (scrollRef.value) {
    scrollRef.value.scrollLeft = scrollRef.value.scrollWidth; // к последней неделе
    updateFades();
    scrollRef.value.addEventListener('scroll', updateFades, {passive: true});
  }
});
watch(
  () => props.data.length,
  async () => {
    render();
    await nextTick();
    if (scrollRef.value) {
      scrollRef.value.scrollLeft = scrollRef.value.scrollWidth;
      updateFades();
    }
  },
  {immediate: true}
);
</script>

<template>
  <!-- скрытая палитра -->
  <div ref="colorStepsRef" class="hidden">
    <div v-for="c in colorClasses" :key="c" :class="c"></div>
  </div>

  <!-- layout: фиксированная лев. колонка + скролл с fade‑масками -->
  <div class="flex select-none">
    <!-- левая колонка -->
    <svg ref="leftSvgRef" class="flex-none"></svg>

    <!-- скролл‑блок -->
    <div
      ref="scrollRef"
      class="grid-wrapper flex-1 overflow-x-auto overflow-y-hidden relative"
      :class="{ 'fade-left': fadeLeft, 'fade-right': fadeRight }"
    >
      <svg ref="gridSvgRef"></svg>
    </div>
  </div>
</template>

<style scoped>
svg {
  display: block;
}

.cell {
  cursor: pointer;
}

.cell:hover,
.cell:focus {
  stroke: #000 !important;
  stroke-width: 2 !important;
}

.month {
  font-weight: bold;
}

/* ——— Fade‑маска для горизонтального скролла ——— */
.grid-wrapper {
  scrollbar-width: none;
  position: relative;
  /* ширина полупрозрачной зоны */
  --fade-size: 16px;
  /* по умолчанию маска прозрачна */
  --left-fade: 0px;
  --right-fade: 0px;

  /* маска скрывает содержимое влево/вправо постепенно */
  --mask: linear-gradient(
    to right,
    transparent 0,
    black var(--left-fade),
    black calc(100% - var(--right-fade)),
    transparent 100%
  );

  -webkit-mask-image: var(--mask);
  mask-image: var(--mask);
  /* для старых Safari, чтобы маска оставалась при скролле */
  -webkit-mask-clip: border-box;
  mask-clip: border-box;
}

/* включаем левый фейд */
.fade-left {
  --left-fade: var(--fade-size);
}

/* включаем правый фейд */
.fade-right {
  --right-fade: var(--fade-size);
}
</style>
