<template>
  <Card :class="['bible-book-card', stateClass]">
    <!-- Шапка карточки -->
    <template #title>
      <h3 class="book-title" :title="bookTitle">{{ bookTitle }}</h3>
    </template>
    <!-- Основное содержимое -->
    <template #content>
      <div class="card-content">
        <div class="progress-text">{{ readChapters }} / {{ totalChapters }}</div>
        <!-- Кастомный прогрессбар -->
        <div class="custom-progress-bar">
          <div class="progress-fill" :style="{ width: progressValue + '%' }"></div>
        </div>
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { withDefaults, defineProps } from 'vue';

interface Props {
  bookTitle: string;
  totalChapters: number;
  readChapters: number;
}

const props = withDefaults(defineProps<Props>(), {
  bookTitle: 'Бытие',
  totalChapters: 153,
  readChapters: 70
});

// Вычисляем процент прочтения
const progressValue = computed(() => {
  if (props.totalChapters === 0) return 0;
  return (props.readChapters / props.totalChapters) * 100;
});

// Определяем состояние карточки
const state = computed(() => {
  if (props.readChapters === 0) {
    return 'not-started';
  } else if (props.readChapters >= props.totalChapters) {
    return 'finished';
  } else {
    return 'in-progress';
  }
});

// Привязываем CSS-классы в зависимости от состояния
const stateClass = computed(() => ({
  'not-started': state.value === 'not-started',
  'in-progress': state.value === 'in-progress',
  'finished': state.value === 'finished'
}));
</script>

<style scoped>
/* Карточка */
.bible-book-card {
  width: 140px;
  position: relative;
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
  text-align: center;
  transition: box-shadow 0.3s;
}
.bible-book-card:hover {
  box-shadow: 0 6px 14px rgba(0, 0, 0, 0.07);
}

/* Свечение для завершенных карточек */
.finished {
  box-shadow: 0 0 10px 2px rgba(76, 175, 80, 0.4);
}

/* Заголовок книги */
.book-title {
  margin: 0;
  padding-top: 8px;
  font-size: 1rem;
  color: #2e3c5d;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Содержимое карточки */
.card-content {
  padding: 8px 12px 12px;
}

.progress-text {
  margin-bottom: 8px;
  font-size: 0.85rem;
  color: #555;
}

/* Кастомный прогрессбар */
.custom-progress-bar {
  width: 100%;
  height: 8px;
  background-color: #f2f2f2;
  border-radius: 6px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  width: 0;
  transition: width 0.3s;
  border-radius: 6px;
}

/* Состояния прогрессбара */
.not-started .progress-fill {
  background-color: #bdbdbd;
}
.in-progress .progress-fill {
  background-color: #2196f3;
}
.finished .progress-fill {
  background-color: #4caf50;
}

/* Состояние "не начато": делаем текст серым */
.not-started .book-title,
.not-started .progress-text {
  color: #9e9e9e;
}
</style>
