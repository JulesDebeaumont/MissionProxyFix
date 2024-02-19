import { defineStore } from 'pinia';
import { Cookies } from 'quasar';
import { api } from 'src/boot/axios';
import { ITokenDecode, IUser } from 'src/components/models';
import jwt_decode from 'jwt-decode';

const COOKIE_JWT = 'mission-dev-jwt';
const COOKIE_JWT_REFRESH = 'mission-dev-refresh-token';
const COOKIE_ZIMBRA_AUTH_TOKEN = 'mission-dev-zimbra-auth-token';
const SAME_SITE = 'Strict';
const COOKIE_PATH = '/';

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: <IUser['id'] | null>null,
    userFullName: <IUser['fullname'] | null>null,
    roles: <IUser['roles']>[],
    tokenExpirationTimestamp: <number | null>null,
    hasZimbraAuthToken: <boolean>false,
  }),
  getters: {
    tokenExpire(): boolean {
      if (this.tokenExpirationTimestamp !== null) {
        return new Date() > new Date(this.tokenExpirationTimestamp * 1000);
      }
      return false;
    },
    isUserConnected(): boolean {
      return this.userId !== null;
    },
    jwtInCookie(): boolean {
      return Cookies.has(COOKIE_JWT);
    },
    refreshTokenInCookie(): boolean {
      return Cookies.has(COOKIE_JWT_REFRESH);
    },
    zimbraAuthTokenInCookie(): boolean {
      return Cookies.has(COOKIE_ZIMBRA_AUTH_TOKEN);
    },
    getJwtInCookie(): string {
      return Cookies.get(COOKIE_JWT);
    },
    getRefreshTokenInCookie(): string {
      return Cookies.get(COOKIE_JWT_REFRESH);
    },
    getZimbraAuthTokenInCookie(): string {
      return Cookies.get(COOKIE_ZIMBRA_AUTH_TOKEN);
    },
  },
  actions: {
    setupUserFromJwtInCookies() {
      const token: string = Cookies.get(COOKIE_JWT);
      const refreshToken: string = Cookies.get(COOKIE_JWT_REFRESH);
      this.setupUserInStore(token, refreshToken);
    },
    setupZimbraAuthToken(csrfToken: string) {
      Cookies.set(COOKIE_ZIMBRA_AUTH_TOKEN, csrfToken, {
        sameSite: SAME_SITE,
        path: COOKIE_PATH,
      });
      this.hasZimbraAuthToken = true;
    },
    clear() {
      this.userId = null;
      this.userFullName = null;
      this.roles = [];
      this.tokenExpirationTimestamp = null;
      this.hasZimbraAuthToken = false;
      Cookies.remove(COOKIE_JWT, { path: COOKIE_PATH });
      Cookies.remove(COOKIE_JWT_REFRESH, { path: COOKIE_PATH });
      Cookies.remove(COOKIE_ZIMBRA_AUTH_TOKEN, { path: COOKIE_PATH });
    },
    logoutZimbra() {
      Cookies.remove(COOKIE_ZIMBRA_AUTH_TOKEN, { path: COOKIE_PATH });
      this.hasZimbraAuthToken = false;
    },
    setupUserInStore(token: string, refreshToken: string) {
      const tokenDecode = jwt_decode(token) as ITokenDecode;
      this.userId = tokenDecode.UserId;
      this.userFullName = tokenDecode.UserFullname;
      this.roles = tokenDecode.UserRoles;
      this.tokenExpirationTimestamp = tokenDecode.exp;
      Cookies.set(COOKIE_JWT, token, {
        sameSite: SAME_SITE,
        path: COOKIE_PATH,
      });
      Cookies.set(COOKIE_JWT_REFRESH, refreshToken, {
        sameSite: SAME_SITE,
        path: COOKIE_PATH,
      });
    },
    async getJwt(username: string, password: string) {
      try {
        const responseJwt = await api.post('login', { username, password });
        this.setupUserInStore(
          responseJwt.data.encodedJwtToken,
          responseJwt.data.refreshToken
        );
      } catch (error: any) {
        console.error(error);
      }
    },
  },
});
