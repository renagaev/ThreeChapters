<template>
  <div class="contact-list">
    <div v-for="(user, index) in users" :key="user.id">
      <Button>{{user.name}}</Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useStore } from '@/store'
import { computed, onMounted } from 'vue'
import {Button} from "@/components/ui/button";

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
