<script setup lang="ts">
import * as d3 from 'd3';
import {ref, onMounted, onBeforeUnmount, watch} from 'vue';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';

dayjs.locale('ru');

export interface Record {
  date: Date;
  count: number;
}

/* ——— Параметры визуала ——— */
const CELL = 16;   // размер ячейки
const GAP = 3;    // промежуток между ячейками
const RADIUS = 3;    // скругление углов
const BLOCK_GAP = 18;   // промежуток между блоками (для подписей месяцев)
const MARGIN_TOP = BLOCK_GAP;
const MARGIN_L = 15;   // левый отступ под дни недели

/* ——— Props / Emit ——— */
const props = defineProps<{ data: Record[] }>();
const emit = defineEmits<{ (e: 'clicked', p: { date: number, count: number | null }): void }>();

/* ——— Палитра (tailwind-цвета) ——— */
const colorClasses = ['bg-gray-400', 'bg-gray-600', 'bg-gray-800', 'bg-primary'];
const colorStepsRef = ref<HTMLDivElement>();
const emptyColor = 'rgba(229,231,235,1)';

function getColors() {
  return Array.from(colorStepsRef.value!.children)
    .map(el => getComputedStyle(el as HTMLElement).backgroundColor);
}

/* ——— Refs и ResizeObserver ——— */
const containerRef = ref<HTMLDivElement>();
const svgRef = ref<SVGSVGElement>();
let resizeObserver: ResizeObserver | null = null;

/** Считает, сколько недель влезает по ширине контейнера */
function computeMaxCols(): number {
  const w = containerRef.value?.clientWidth ?? window.innerWidth;
  const gridW = Math.max(0, w - MARGIN_L);
  return Math.max(1, Math.floor((gridW + GAP) / (CELL + GAP)));
}

/** Основная функция рендера */
function render() {
  if (!svgRef.value) return;
  const maxCols = computeMaxCols();

  // 1) формируем map<деньUTC, count>
  const map = new Map<number, number>();
  props.data.forEach(r =>
    map.set(dayjs(r.date).startOf('day').valueOf(), r.count)
  );

  // 2) собираем все дни от первой недели до сегодня
  const minTs = Math.min(...map.keys());
  const firstWeekStart = dayjs(minTs).startOf('week');
  const today = dayjs().startOf('day');

  type Cell = { ts: number; count: number | null; x: number; y: number; wrap: number };
  const days: Cell[] = [];
  for (let d = firstWeekStart, i = 0;
       d.isBefore(today) || d.isSame(today);
       d = d.add(1, 'day'), i++
  ) {
    const weekIdx = Math.floor(i / 7);
    const wrap = Math.floor(weekIdx / maxCols);
    const col = weekIdx % maxCols;
    const rowInBlk = i % 7;
    days.push({
      ts: d.valueOf(),
      count: map.get(d.valueOf()) ?? null,
      x: MARGIN_L + col * (CELL + GAP),
      y: MARGIN_TOP + wrap * (7 * (CELL + GAP) + BLOCK_GAP) + rowInBlk * (CELL + GAP),
      wrap
    });
  }
  const wraps = d3.max(days, d => d.wrap)! + 1;

  // 3) размеры SVG
  const gridW = maxCols * (CELL + GAP) - GAP;
  const gridH = wraps * (7 * (CELL + GAP) + BLOCK_GAP) - GAP - BLOCK_GAP;
  const svgW = MARGIN_L + gridW;
  const svgH = MARGIN_TOP + gridH;

  // 4) цветовая шкала
  const counts = props.data.map(r => r.count);
  const colorScale = d3.scaleQuantize<string>()
    .domain([d3.min(counts)!, d3.max(counts)!])
    .range(getColors());

  // 5) инициализируем SVG
  const svg = d3.select(svgRef.value)
    .attr('width', svgW)
    .attr('height', svgH)
    .attr('viewBox', `0 0 ${svgW} ${svgH}`)
    .style('font-family', 'sans-serif')
    .style('font-size', '9px');
  svg.selectAll('*').remove();
  const g = svg.append('g');

  // 6) рендер ячеек
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
    .attr('x', d => d.x).attr('y', d => d.y)
    .attr('width', CELL).attr('height', CELL)
    .attr('rx', RADIUS).attr('ry', RADIUS)
    .attr('fill', d => d.count === null ? emptyColor : colorScale(d.count))
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

  // 7) подписываем месяцы без наложений
  type Label = { wrap: number; x: number; text: string; w: number };
  // raw-labels и замер ширины
  const raw: Label[] = [];
  const seen = new Set<string>();
  days.forEach(d => {
    const m = dayjs(d.ts).month();
    const key = `${d.wrap}-${m}`;
    if (!seen.has(key)) {
      raw.push({
        wrap: d.wrap,
        x: d.x + 2,
        text: dayjs(d.ts).format('MMM'),
        w: 0
      });
      seen.add(key);
    }
  });
  // временно замеряем каждую
  const measure = g.append('g').attr('class', '_measure');
  measure.selectAll('text')
    .data(raw)
    .enter()
    .append('text')
    .text(d => d.text)
    .each(function (d) {
      d.w = (this as SVGTextElement).getBBox().width;
    });
  measure.remove();

  // группируем по wrap и фильтруем overlap
  const grouped = d3.group(raw, d => d.wrap);
  const finals: Label[] = [];
  for (const [wrap, labels] of grouped) {
    labels.sort((a, b) => a.x - b.x);
    const keep: Label[] = [];
    labels.forEach(l => {
      if (!keep.length) keep.push(l);
      else {
        const last = keep[keep.length - 1];
        if (l.x <= last.x + last.w + 4) {
          // перекрывается — заменяем «старую» на «новую»
          keep[keep.length - 1] = l;
        } else {
          keep.push(l);
        }
      }
    });
    finals.push(...keep);
  }

  // render final month labels
  const monthSel = g.selectAll<SVGTextElement, Label>('text.month')
    .data(finals, (d: Label) => `${d.wrap}-${d.text}`);

  // remove old labels
  monthSel.exit().remove();

  // append new labels
  const monthEnter = monthSel.enter()
    .append('text')
    .attr('class', 'month')
    .text((d: Label) => d.text);

  // merge and set position
  monthEnter.merge(monthSel)
    .attr('x', (d: Label) => d.x)
    .attr('y', (d: Label) => MARGIN_TOP + d.wrap * (7 * (CELL + GAP) + BLOCK_GAP) - BLOCK_GAP / 3);

  // 8) подписи дней недели слева
  const weekDays = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
  for (let w = 0; w < wraps; w++) {
    weekDays.forEach((lbl, i) => {
      g.append('text')
        .attr('x', MARGIN_L - 15)
        .attr('y', MARGIN_TOP + w * (7 * (CELL + GAP) + BLOCK_GAP) + i * (CELL + GAP) + 12)
        .text(lbl);
    });
  }
}

/* Жизненный цикл + ResizeObserver */
onMounted(() => {
  render();
  if (containerRef.value) {
    resizeObserver = new ResizeObserver(render);
    resizeObserver.observe(containerRef.value);
  }
});
onBeforeUnmount(() => {
  if (resizeObserver && containerRef.value) {
    resizeObserver.unobserve(containerRef.value);
  }
});
watch(() => props.data.length, render, {immediate: true});
</script>

<template>
  <!-- скрытая палитра для чтения Tailwind-цветов -->
  <div ref="colorStepsRef" class="hidden">
    <div v-for="c in colorClasses" :key="c" :class="c"></div>
  </div>

  <!-- контейнер, по ширине которого считается grid -->
  <div ref="containerRef" class="overflow-auto">
    <svg ref="svgRef" class="overflow-visible"></svg>
  </div>
</template>

<style scoped>
svg {
  display: block;
}

/* убирать любой UA-outline, и рисовать только нашу чёрную */
svg, svg *:focus {
  outline: none !important;
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
</style>
