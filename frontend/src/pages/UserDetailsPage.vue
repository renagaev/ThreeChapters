<script setup lang="ts">
import BibleProgress from "@/components/bibleprogress/BibleProgress.vue";
import {ref, onBeforeMount} from "vue";
import {useStore, type UserDetails} from "@/store";
import {Separator} from "@/components/ui/separator";
import {useRouter} from "vue-router";

const props = defineProps({
  userId: {
    type: Number,
    required: true
  }
})

const user = ref<UserDetails>({
  id: 0,
  name: "",
  memberFrom: new Date()
})

const store = useStore()
const router = useRouter()

onBeforeMount(async () => {
  user.value = await store.fetchUserDetails(props.userId)
})

function goBack() {
  router.push("/users")
}
</script>

<template>
  <div>
    <div class="p-4 bg-white space-y-4">
      <!-- Кнопка назад -->
      <button
        @click="goBack"
        class="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-md"
      >
        Назад
      </button>

      <!-- Информация о пользователе -->
      <div>
        <h1 class="text-2xl font-bold text-gray-800">{{ user.name }}</h1>
        <p class="text-sm text-gray-600">
          Участник с {{ user.memberFrom.toLocaleDateString() }}
        </p>
      </div>
    </div>
    <Separator/>

    <!-- Компонент с прогрессом чтения -->
    <BibleProgress :user-id="props.userId"/>
  </div>
</template>

<style scoped>
/* При необходимости добавьте дополнительные стили */
</style>
