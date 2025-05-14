import {createRouter, createWebHistory, type RouteRecordRaw} from 'vue-router';
import UsersPage from "@/pages/UsersPage.vue";
import UserDetailsPage from "@/pages/UserDetailsPage.vue";
import {retrieveLaunchParams} from '@telegram-apps/sdk';
import {UserService} from "@/client";

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'index',
    component: UsersPage,
    beforeEnter: async (to, from, next) => {
      const {initDataRaw, initData} = retrieveLaunchParams();
      if (initData?.user?.id) {
        const userId = await UserService.getUserIdByTelegramId(initData?.user?.id)
        if (userId) {
          next({name: 'user', params: {userId}})
          return
        }
      }
      next()
    }
  },
  {
    path: '/users',
    name: 'users',
    component: UsersPage,
  },
  {
    path: '/user/:userId',
    name: 'user',
    component: UserDetailsPage,
    props: (route) => ({userId: Number(route.params.userId)})
  },
  {
    path: '/:catchAll(.*)',
    name: 'not-found',
    redirect: {name: 'index'},
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    return {top: 0}
  }
})

export default router;
