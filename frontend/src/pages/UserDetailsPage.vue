<script setup lang="ts">
import BibleProgress from "@/components/bibleprogress/BibleProgress.vue";

const props = defineProps({userId: {type: Number, required: true}})


</script>

<template>
  <BibleProgress :user-id="props.userId!"/>
</template>

<style scoped>

</style>
