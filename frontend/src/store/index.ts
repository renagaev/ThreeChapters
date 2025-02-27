import {defineStore} from "pinia";
import type {StructureTestament} from "@/client";
import {BibleService, UserService} from "@/client";
import {ref} from "vue";

export interface User {
  id: number;
  name: string;
}

export interface UserDetails {
  id: number;
  name: string;
  memberFrom: Date;
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

  async function fetchUserReadChapters(userId: number) {
    const res = await UserService.getUserReadChapters(userId);
    return res.reduce((acc, cur) => {
      acc.set(cur.bookId!, cur.chapters!)
      return acc
    }, new Map<number, number[]>());

  }

  async function fetchUserDetails(userId: number): Promise<UserDetails> {
    const res = await UserService.getUserDetails(userId)
    return {id: res.id!, name: res.name!, memberFrom: new Date(res.memberFrom!)} as UserDetails
  }

  return {users, fetchUsers, bibleStructure, fetchBibleStructure, fetchUserReadChapters, fetchUserDetails}
})
