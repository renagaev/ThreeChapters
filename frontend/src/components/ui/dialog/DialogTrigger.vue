<script setup lang="ts">
import {DialogTrigger, type DialogTriggerProps} from 'reka-ui'

const props = defineProps<DialogTriggerProps & { disabled?: boolean }>()
</script>

<template>
  <DialogTrigger v-if="!props.disabled" v-bind="props">
    <slot/>
  </DialogTrigger>
  <slot v-else/>
</template>
