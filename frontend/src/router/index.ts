import {createRouter, createWebHashHistory} from 'vue-router';
import UsersPage from "@/pages/UsersPage.vue";
import UserDetailsPage from "@/pages/UserDetailsPage.vue";

export const routes = [
  {
    path: '/',
    name: 'index',
    component: UsersPage,
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
    // @ts-ignore
    props: (route: unknown) => ({userId: Number(route.params.userId)})
  }
];

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
