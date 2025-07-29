<script setup lang="ts">
import BibleProgress from "@/components/bibleprogress/BibleProgress.vue";
import {ref, onBeforeMount, computed} from "vue";
import {useStore, type UserDetails} from "@/store";
import {Separator} from "@/components/ui/separator";
import {useRouter} from "vue-router";
import UserReadCalendar from "@/components/UserReadCalendar.vue";
import {Avatar, AvatarImage} from "@/components/ui/avatar";
import ReadProgress from "@/components/stats/BibleReadProgress.vue";
import ReadTimes from "@/components/stats/ReadTimes.vue";
import Streaks from "@/components/stats/Streaks.vue";
import HabitPower from "@/components/stats/HabitPower.vue";

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
  await Promise.all([
    store.fetchUserDetails(props.userId).then(details => {
      user.value = details;
    }),
    store.fetchUserBibleReadingStats(props.userId),
    store.fetchUserStreaks(props.userId),
    store.fetchUserHabitPower(props.userId)
  ]);
});
const bibleProgress = computed(() => store.bibleReadProgress!)


function goBack() {
  router.push("/users")
}
</script>

<template>
  <div>
    <div class="p-3 bg-white space-y-4">
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
    <div class="grid grid-cols-2 md:grid-cols-3 gap-3 p-3">
      <HabitPower :current="store.habitPower.current"/>
      <ReadProgress :percentage="bibleProgress.currentPercentage"/>
      <Streaks :days="store.streaks.current ?? 1"/>
      <ReadTimes v-if="bibleProgress.readTimes != 0" :times="bibleProgress.readTimes"/>
    </div>
    <Separator/>
    <user-read-calendar :user-id="userId"/>
    <Separator/>

    <BibleProgress :read-chapters="bibleProgress.chaptersRead"/>
  </div>
</template>

<style scoped>
/* При необходимости добавьте дополнительные стили */
</style>
