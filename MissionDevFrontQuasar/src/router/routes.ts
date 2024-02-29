import { RouteRecordRaw } from 'vue-router';
import {
  gateIsUserAuthenticated,
  gateIsUserNotAuthenticated,
} from 'src/router/gates';

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
    redirect: { name: 'dashboard' },
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    redirect: { name: 'myProjects' },
    beforeEnter: gateIsUserAuthenticated,
    children: [
      {
        path: 'documents',
        name: 'documents',
        component: () => import('pages/document/MyDocumentPage.vue'),
      },
      {
        path: 'projects',
        children: [
          {
            path: 'my-projects',
            name: 'myProjects',
            component: () => import('pages/project/UserProjectPage.vue'),
          },
          {
            path: ':projectId',
            name: 'projectShow',
            component: () => import('pages/project/ProjectPage.vue'),
          },
          {
            path: 'edit/:projectId',
            name: 'projectEdit',
            component: () => import('pages/project/ProjectEditPage.vue'),
          },
          {
            path: 'create',
            name: 'projectCreate',
            component: () => import('pages/project/ProjectEditPage.vue'),
          },
        ],
      },
    ],
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
