import { useUserStore } from 'src/stores/user-store';

export function gateIsUserAuthenticated() {
  const userStore = useUserStore();
  if (userStore.jwtInCookie) {
    userStore.setupUserFromJwtInCookies();
  }
  if (!userStore.isUserConnected) {
    return { name: 'login' };
  }
}
export function gateIsUserNotAuthenticated() {
  const userStore = useUserStore();
  if (userStore.jwtInCookie) {
    userStore.setupUserFromJwtInCookies();
  }
  if (userStore.isUserConnected) {
    return { name: 'dashboard' };
  }
}
