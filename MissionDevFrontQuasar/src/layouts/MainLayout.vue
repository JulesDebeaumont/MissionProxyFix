<script setup lang="ts">
import { ref } from 'vue';
import { useUserStore } from 'src/stores/user-store';
import { useRouter } from 'vue-router';
import { api } from 'src/boot/axios';
import { exportFile } from 'quasar';
// components
import SideBarMenu from 'src/components/layout/SideBarMenu.vue';
import ApplicationTitle from 'src/components/layout/ApplicationTitle.vue';

// consts
const userStore = useUserStore();
const router = useRouter();

// refs
const test = ref();
const test2 = ref();
const uploadFileRed = ref();
const leftDrawer = ref(true);

// functions
function toggleLeftDrawer() {
  leftDrawer.value = !leftDrawer.value;
}
async function testGetRefreshToken() {
  const responseUser = await api.post('refresh-token', {
    Jwt: userStore.getJwtInCookie,
    RefreshToken: userStore.getRefreshTokenInCookie,
  });
  test.value = responseUser.data;
}
async function testPolicy() {
  const responseUser = await api.get('users/adjime');
  test2.value = responseUser.data;
}
async function testUpload() {
  const formData = new FormData();
  formData.append('files', uploadFileRed.value);
  await api.post('users/yeah', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
async function testDownload() {
  const file = (await api.post('users/oula2/10')).data;
  exportFile('sdfsdf.json', file);
}
async function testEraseFile() {
  await api.post('users/oula3/10');
}
function logout() {
  userStore.clear();
  router.push({ name: 'login' });
}
</script>

<template>
  <q-layout view="hHh Lpr lff">
    <q-header class="bg-accent" height-hint="64" elevated>
      <q-toolbar class="GPL__toolbar" style="height: 64px">
        <q-btn
          @click="toggleLeftDrawer"
          :disable="!userStore.isUserConnected"
          flat
          dense
          round
          icon="menu"
          aria-label="Menu"
          class="q-mx-md"
        />
        <q-toolbar-title shrink class="row items-center no-wrap">
          <ApplicationTitle class="q-pt-sm" />
        </q-toolbar-title>

        <q-space />

        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn-dropdown :disable="!userStore.isUserConnected" round flat>
            <template v-slot:label>
              <q-avatar size="26px" icon="person" color="primary" />
            </template>

            <q-list>
              <q-item>
                <q-item-section>
                  {{ userStore.userFullName }}
                </q-item-section>
              </q-item>

              <q-separator />

              <q-item clickable v-close-popup @click="logout()">
                <q-item-section avatar>
                  <q-icon name="logout" />
                </q-item-section>
                <q-item-section>
                  <q-item-label>Déconnexion</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
        </div>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-if="userStore.isUserConnected"
      v-model="leftDrawer"
      bordered
      :width="230"
      :breakpoint="500"
      show-if-above
      class="bg-accent"
    >
      <div class="flex flex-center column full-height">
        <q-list
          class="flex justify-start items-center column no-wrap q-gutter-y-md full-height"
        >
          <SideBarMenu />
        </q-list>
      </div>

      <!-- <q-list>
        <q-item-label header> Projects </q-item-label>
        <pre>{{ userStore.test() }}</pre>
        <q-btn @click="testGetRefreshToken" label="Refresh token" />
        {{ test }}

        <q-btn @click="testPolicy" label="Policy" />
        {{ test2 }}

        <q-file
          clearable
          filled
          color="purple-12"
          v-model="uploadFileRed"
          label="Label"
        />
        <q-btn @click="testUpload" label="upload" />
        <q-btn @click="testDownload" label="download" />
        <q-btn @click="testEraseFile" label="erase filee" />
        <EssentialLink
          v-for="project in projects"
          :key="project.ID"
          :project="project"
        />
      </q-list> -->
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
