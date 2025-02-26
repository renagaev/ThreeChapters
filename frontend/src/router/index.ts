import { createRouter, createWebHistory } from 'vue-router';
import UsersPage from "@/pages/UsersPage.vue";

export const routes = [
  {
    path: '/',
    name: 'index',
    component: UsersPage,
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
