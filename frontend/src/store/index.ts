import {defineStore} from "pinia";
import type {StructureTestament} from "@/client";
import {BibleService, UserService} from "@/client";
import {ref} from "vue";

export interface User {
  id: number;
  name: string;
}

export const useStore = defineStore('store', () => {
  const users = ref(Array.of<User>())
  const bibleStructure = ref(Array.of<StructureTestament>())

  async function fetchUsers() {
    const res = await UserService.getUsers()
    users.value = res.map(u => u as User)
  }

  async function fetchBibleStructure() {
    if (bibleStructure.value.length === 0) {
      bibleStructure.value = await BibleService.getStructure()
    }
  }

  return {users, fetchUsers, bibleStructure, fetchBibleStructure}
})
