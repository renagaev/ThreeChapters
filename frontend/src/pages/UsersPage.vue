<template>
  <CalendarHeatmap :data="data"></CalendarHeatmap>
  <UsersList/>
</template>

<script setup lang="ts">
const data = Array.from(Array(29).keys()).map(((i) => ({
  date: new Date(2025, 1, i + 1).toISOString().slice(0, 10),
  count: Math.floor(Math.random() * 10)
})));
import UsersList from "@/components/UsersList.vue";
import CalendarHeatmap from "@/components/CalendarHeatmap.vue";


</script>

<style scoped>
</style>
