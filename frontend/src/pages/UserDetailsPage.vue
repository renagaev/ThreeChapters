<script setup lang="ts">
import BibleProgress from "@/components/bibleprogress/BibleProgress.vue";
import {ref, onBeforeMount} from "vue";
import {useStore, type UserDetails} from "@/store";
import {Separator} from "@/components/ui/separator";
import {useRouter} from "vue-router";
import UserReadCalendar from "@/components/UserReadCalendar.vue";
import {Avatar, AvatarImage} from "@/components/ui/avatar";

const props = defineProps({
  userId: {
    type: Number,
    required: true
  }
})

const user = ref<UserDetails>({
  id: 0,
  name: "",
  memberFrom: new Date(),
  hasAvatar: false,
  avatarUrl: ""
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
        class="px-3 py-1 bg-gray-100 text-gray-700 rounded-md"
      >
        К списку
      </button>

      <div class="flex items-center">
        <div v-if="user.hasAvatar" class="mr-4">
          <Avatar class="w-28 h-28  ">
            <AvatarImage :src="user.avatarUrl" class="border"></AvatarImage>
          </Avatar>
        </div>
        <div>
          <h1 class="text-4xl font-bold text-gray-800">{{ user.name }}</h1>
          <p class="text-normal text-gray-600">
            Участник с {{ user.memberFrom.toLocaleDateString() }}
          </p>
        </div>
      </div>



    </div>
    <Separator/>
    <user-read-calendar :user-id="userId"/>
    <Separator/>

    <!-- Компонент с прогрессом чтения -->
    <BibleProgress :user-id="props.userId"/>
  </div>
</template>

<style scoped>
/* При необходимости добавьте дополнительные стили */
</style>
