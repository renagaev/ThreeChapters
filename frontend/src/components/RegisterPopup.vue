<script setup lang="ts">
import {useStore} from "@/store";
import {onBeforeMount, ref} from "vue";
import {Dialog, DialogContent, DialogTitle} from "@/components/ui/dialog";
import {Input} from "@/components/ui/input";
import {Button} from "@/components/ui/button";
import {retrieveLaunchParams} from '@telegram-apps/sdk-vue'
import {useRouter} from "vue-router";
import {Icon} from "@iconify/vue";

const store = useStore()
const router = useRouter()
const params = retrieveLaunchParams()

const name = ref("")
const open = ref(false)
const loading = ref(false)

onBeforeMount(async () => {
  await store.fetchCurrentUser()
  open.value = !store.currentUser
})


const tgName = params?.initData?.user?.firstName
const register = async () => {
  loading.value = true
  const userId = await store.registerUser(name.value)
  open.value = false
  loading.value = false
  await router.push({name: "user", params: {userId}})
}

</script>

<template>
  <Dialog :open="open">
    <DialogContent class="p-4 w-5/6 rounded-md" hide-close-button>
      <DialogTitle class="text-xl font-semibold">
        Привет{{ !!tgName ? ", " + tgName : "" }}! <br>Давай читать Библию вместе
      </DialogTitle>
      <div class="text-sm">
        1. Введи свое имя<br/>
        2. Нажми "Начать"<br/>
        3. Прочитай несколько глав из Библии<br/>
        4. Оставь комментарий c прочитанными главами к посту в канале<br/>
      </div>

      <div class="grid gap-3">
        <Input
          id="name"
          placeholder="Твое имя"
          v-model="name"
          class="w-full mt-2"
          @keyup.enter="register"
        />

        <Button class="w-full" @click="register" :disabled="!name.trim()">
          <Icon v-if="loading" :icon="'tabler:loader-2'" :inline="true" class="animate-spin"/>
          <div v-else>Начать</div>
        </Button>

        <Button :variant="'secondary'" class="w-full" @click="open = false">Позже</Button>
      </div>
    </DialogContent>
  </Dialog>
</template>

<style scoped>

</style>
