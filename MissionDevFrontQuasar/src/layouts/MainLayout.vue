<script setup lang="ts">
import { ref } from 'vue';
import { useUserStore } from 'src/stores/user-store';
import { useRouter } from 'vue-router';
import { IProject } from 'src/components/models';
import { api } from 'src/boot/axios';
// components
import EssentialLink from 'src/components/EssentialLink.vue';

// consts
const userStore = useUserStore();
const router = useRouter();

// refs
const leftDrawerOpen = ref(false);
const projects = ref<IProject[]>([]);
const test = ref();
const test2 = ref();
const uploadFileRed = ref();

// functions
function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}
function logout() {
  userStore.purge();
  router.push({ name: 'login' });
}
async function testEndPoint() {
  const responseUser = await api.post('refresh-token', {
    Jwt: userStore.getJwtInCookie(),
    RefreshToken: userStore.getRefreshTokenInCookie(),
  });
  test.value = responseUser.data;
}
async function testEndPoint2() {
  const responseUser = await api.get('users/adjime');
  test2.value = responseUser.data;
}
async function testEndPoint3() {
  const formData = new FormData();
  formData.append('files', uploadFileRed.value);
  await api.post('users/yeah', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="menu"
          aria-label="Menu"
          @click="toggleLeftDrawer"
        />

        <q-toolbar-title> Mission Dev </q-toolbar-title>

        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn-dropdown :disable="!userStore.isUserConnected" round flat>
            <template v-slot:label>
              <q-avatar size="26px" icon="person" color="secondary" />
            </template>

            <q-list>
              <q-item clickable v-close-popup @click="logout()">
                <q-item-section>
                  <q-item-label>Déconnexion</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
        </div>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-list>
        <q-item-label header> Projects </q-item-label>
        <pre>{{ userStore.test() }}</pre>
        <q-btn @click="testEndPoint" label="test" />
        {{ test }}

        <q-btn @click="testEndPoint2" label="test" />
        {{ test2 }}

        <q-file
          clearable
          filled
          color="purple-12"
          v-model="uploadFileRed"
          label="Label"
        />
        <q-btn @click="testEndPoint3" label="test" />

        <EssentialLink
          v-for="project in projects"
          :key="project.ID"
          :project="project"
        />
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
