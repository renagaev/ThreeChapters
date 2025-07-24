<script setup lang="ts">

import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";
import {Icon} from "@iconify/vue";

const props = defineProps<{ text: string, icon: string | undefined, title: string }>()

</script>

<template>
  <Card class="w-full p-2 rounded-md border shadow">
    <CardHeader class="flex flex-row items-center justify-between space-y-1 p-2 pb-1">
      <CardTitle class="text-sm font-semibold">
        {{ props.title }}
      </CardTitle>
    </CardHeader>
    <CardContent class="px-2 pb-2">
      <div class="text-2xl font-semibold">
        <Icon v-if="icon !== undefined" :icon="props.icon!" :inline="true" class="inline"/>
        {{ props.text }}
      </div>
    </CardContent>
  </Card>
</template>

<style scoped>

</style>
