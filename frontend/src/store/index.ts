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

export interface BibleReadProgress {
  chaptersRead: Map<number, number[]>
  readTimes: number,
  currentPercentage: number
}
const emptyBibleReadProgress: BibleReadProgress = {
  chaptersRead: new Map<number, number[]>(),
  currentPercentage: 0,
  readTimes: 0
}

export const useStore = defineStore('store', () => {
  const users = ref(Array.of<User>())
  const bibleStructure = ref(Array.of<StructureTestament>())
  const bibleReadProgress = ref<BibleReadProgress>(emptyBibleReadProgress)

  function getAvatarUrl(path: string | null | undefined, id: number) {
    return path ?
      `${OpenAPI.BASE}/api/v1/users/${id}/avatar/${path}`
      : `https://api.dicebear.com/9.x/lorelei-neutral/svg?seed=${id}&scale=120`
  }

  async function fetchUsers() {
    const res = await UserService.getUsers()
    users.value = res.map(u => {
      return {id: u.id!, name: u.name!, avatarUrl: getAvatarUrl(u.avatar, u.id!)}
    })
  }

  async function fetchBibleStructure() {
    if (bibleStructure.value.length === 0) {
      bibleStructure.value = await BibleService.getStructure()
    }
  }

  async function fetchUserBibleReadingStats(userId: number) {
    bibleReadProgress.value = emptyBibleReadProgress
    const res = await UserService.getUserBibleProgress(userId);
    bibleReadProgress.value = {
      chaptersRead: res.readBookChapters!!.reduce((acc, cur) => {
        acc.set(cur.bookId!, cur.chapters!)
        return acc
      }, new Map<number, number[]>()),
      currentPercentage: res.currentPercentage!!,
      readTimes: res.readTimes!!
    }
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
      avatarUrl: getAvatarUrl(res.avatar, res.id!)
    } as UserDetails
  }

  return {
    users,
    fetchUsers,
    bibleStructure,
    fetchBibleStructure,
    fetchUserBibleReadingStats,
    bibleReadProgress,
    fetchUserReadChaptersByDay,
    fetchUserDetails
  }
})
