import { boot } from 'quasar/wrappers';
import { useUserStore } from 'src/stores/user-store';
import { Notify } from 'quasar';
import axios, { AxiosInstance } from 'axios';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $axios: AxiosInstance;
  }
}

// Be careful when using SSR for cross-request state pollution
// due to creating a Singleton instance here;
// If any client changes this (global) instance, it might be a
// good idea to move this instance creation inside of the
// "export default () => {}" function below (which runs individually
// for each client)
const ROUTE_NAME_LOGIN = 'login';
const ROUTE_NAME_ERROR403 = 'error403';
const api = axios.create({
  baseURL: process.env.API_URL,
});

export default boot(({ router }) => {
  const userStore = useUserStore();

  // Requêtes
  api.interceptors.request.use(
    function (config) {
      if (userStore.isUserConnected) {
        if (userStore.tokenExpire === true) {
          userStore.clear();
          return config;
        }
        config.headers.Authorization = `Bearer ${userStore.getJwtInCookie()}`;
      }
      return config;
    },
    function (error) {
      console.error(error.message);
    }
  );

  // Réponses
  api.interceptors.response.use(
    function (response) {
      return response;
    },
    function (error) {
      console.error(error);
      if (error.response?.status === 401) {
        userStore.clear();
        router.push({ name: ROUTE_NAME_LOGIN });
        return;
      }
      if (error.response?.status === 403) {
        router.push({ name: ROUTE_NAME_ERROR403 });
        return;
      }
      if (error.response?.status === 500) {
        Notify.create({
          type: 'negative',
          message: 'Une erreur est survenue',
        });
        return;
      }
    }
  );
});

export { api };
