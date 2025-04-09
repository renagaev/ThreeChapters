import {defineStore} from "pinia";
import type {StructureTestament} from "@/client";
import {BibleService, OpenAPI, UserService} from "@/client";
import {ref} from "vue";

export interface User {
  id: number;
  name: string;
  avatarUrl: string
}

export interface UserDetails {
  id: number;
  name: string;
  memberFrom: Date;
  avatarUrl: string
  hasAvatar: boolean
}

export interface DayChaptersRead {
  date: Date;
  count: number;
}

export const useStore = defineStore('store', () => {
  const users = ref(Array.of<User>())
  const bibleStructure = ref(Array.of<StructureTestament>())

  function getAvatarUrl(path: string, id: number) {
    return path ?
      `${OpenAPI.BASE}/api/v1/users/${id}/avatar/${path}`
      : `https://api.dicebear.com/9.x/lorelei-neutral/svg?seed=${id}&scale=120`
  }

  async function fetchUsers() {
    const res = await UserService.getUsers()
    users.value = res.map(u => {
      return {id: u.id!, name: u.name!, avatarUrl: getAvatarUrl(u.avatar, u.id)}
    })
  }

  async function fetchBibleStructure() {
    if (bibleStructure.value.length === 0) {
      bibleStructure.value = await BibleService.getStructure()
    }
  }

  async function fetchUserReadChaptersByBook(userId: number) {
    const res = await UserService.getUserReadChaptersByBook(userId);
    return res.reduce((acc, cur) => {
      acc.set(cur.bookId!, cur.chapters!)
      return acc
    }, new Map<number, number[]>());
  }

  async function fetchUserReadChaptersByDay(userId: number) {
    const res = await UserService.getUserReadChaptersByDay(userId);
    return res.map(x => ({date: new Date(x.date!), count: x.count}) as DayChaptersRead)
  }

  async function fetchUserDetails(userId: number): Promise<UserDetails> {
    const res = await UserService.getUserDetails(userId)
    return {
      id: res.id!,
      name: res.name!,
      memberFrom: new Date(res.memberFrom!),
      hasAvatar: !!res.avatar,
      avatarUrl: getAvatarUrl(res.avatar, res.id)
    } as UserDetails
  }

  return {
    users,
    fetchUsers,
    bibleStructure,
    fetchBibleStructure,
    fetchUserReadChaptersByBook,
    fetchUserReadChaptersByDay,
    fetchUserDetails
  }
})
