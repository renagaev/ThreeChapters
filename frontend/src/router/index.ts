import {createRouter, createWebHistory} from 'vue-router';
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
    props: (route: unknown) => ({userId: Number(route.params.userId)})
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
