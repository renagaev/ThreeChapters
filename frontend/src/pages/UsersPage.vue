<template>
  <div class="contact-list">
    <div v-for="(user, index) in users" :key="user.id">
      <div class="contact-item">
        <!-- Показываем первую букву имени как «аватар» -->
        <el-avatar>{{ user.name[0] }}</el-avatar>
        <span>{{ user.name }}</span>
      </div>
      <!-- Разделитель между элементами (не выводится после последнего) -->
      <el-divider v-if="index < users.length - 1" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useStore } from '@/store'
import { computed, onMounted } from 'vue'

const store = useStore()

// При монтировании компонента загружаем список пользователей
onMounted(() => {
  store.fetchUsers()
})

// Реактивно получаем массив пользователей из стора
const users = computed(() => store.users)
</script>

<style scoped>
.contact-list {
  /* При желании можно убрать/изменить фоновый цвет и отступы */
  padding: 16px;
}

/* Каждый контакт — одна строка */
.contact-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 0;
}

/* Если нужен дополнительный отступ у разделителя */
.el-divider {
  margin: 0;
}
</style>
