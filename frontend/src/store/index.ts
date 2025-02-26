import {defineStore} from "pinia";
import {UserService} from "@/client";
import {ref} from "vue";

export interface User {
  id: number;
  name: string;
}

export const useStore = defineStore('store', () => {
  const users = ref(Array.of<User>())

  async function fetchUsers() {
    const res = await UserService.getUsers()
    users.value = res.map(u => u as User)
  }

  return {users, fetchUsers}
})
