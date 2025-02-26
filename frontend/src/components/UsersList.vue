<script setup lang="ts">
import {useStore} from '@/store'
import {computed, onMounted} from 'vue'
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";

const store = useStore()

// При монтировании компонента загружаем список пользователей
onMounted(() => {
  store.fetchUsers()
})
const users = computed(() => store.users)

function getAvatarUrl(name: string) {
  return `https://api.dicebear.com/9.x/lorelei-neutral/svg?seed=${encodeURIComponent(name)}&scale=120`
}
</script>

<template>
  <div>
    <div
      v-for="user in users"
      :key="user.id"
      class="flex items-center space-x-3 p-2 border"
    >
      <Avatar class="w-9 h-9 border border-black rounded-full overflow-hidden">
        <AvatarImage :src="getAvatarUrl(user.name)" :alt="`Avatar of ${user.name}`"/>
        <AvatarFallback>{{ user.name.charAt(0) }}</AvatarFallback>
      </Avatar>
      <div>
        <p class="text-base  font-medium">{{ user.name }}</p>
      </div>
    </div>
  </div>
</template>


<style scoped>

</style>
