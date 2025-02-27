<script setup lang="ts">
import BookCard from "@/components/bibleprogress/BookCard.vue";
import {computed, onBeforeMount} from "vue";
import {useStore} from "@/store";

const store = useStore()
onBeforeMount(() => store.fetchBibleStructure())
const structure = computed(() => store.bibleStructure)


</script>

<template>
  <div class="p-4 bg-gray-50 min-h-screen">
    <div
      v-for="testament in structure"
      :key="testament.title"
      class="mb-8"
    >
      <h1 class="text-xl font-bold text-gray-800 mb-4">
        {{ testament.title }}
      </h1>
      <div
        v-for="group in testament.groups"
        :key="group.title"
        class="mb-6"
      >
        <h2 class="text-lg font-semibold text-gray-700 mb-3">
          {{ group.title }}
        </h2>
        <div class="grid grid-cols-2 md:grid-cols-3 gap-3">
          <BookCard
            v-for="book in group.books"
            :key="book.id"
            :read-chapters="1"
            :total-chapters="book.chaptersCount!"
            :book-name="book.title!"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
