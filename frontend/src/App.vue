<script setup lang="ts">
import {useBackButton} from '@/composables/useBackButton'
import {onBeforeMount} from "vue";
import {useStore, type UserDetails} from "@/store";
import RegisterPopup from "@/components/RegisterPopup.vue";

useBackButton()

const store = useStore()
onBeforeMount(async () => {
  await store.fetchCurrentUser()
})
</script>

<template>
  <div>
    <RegisterPopup/>
    <RouterView/>
  </div>

</template>
