import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/tasks' },
    { path: '/tasks', component: () => import('@/features/tasks/TasksView.vue') },
    { path: '/categories', component: () => import('@/features/categories/CategoriesView.vue') },
    // TODO (Week 3): /login route + a beforeEach guard for authenticated routes
  ],
})

export default router
