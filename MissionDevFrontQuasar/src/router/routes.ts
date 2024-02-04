import { RouteRecordRaw } from 'vue-router';
import { gateIsUserAuthenticated, gateIsUserNotAuthenticated } from 'src/router/gates';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    beforeEnter: gateIsUserNotAuthenticated,
    component: () => import('pages/user/LoginPage.vue'),
  },
  {
    path: '/',
    name: 'root',
    redirect: { name: 'dashboard'}
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    beforeEnter: gateIsUserAuthenticated,
    component: () => import('pages/DashboardPage.vue'),
  },

  
  {
    path: '/error401',
    name: 'error401',
    component: () => import('pages/misc/Error401.vue'),
  },
  {
    path: '/error403',
    name: 'error403',
    component: () => import('pages/misc/Error403.vue'),
  },
  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/misc/ErrorNotFound.vue'),
  },
];

export default routes;
