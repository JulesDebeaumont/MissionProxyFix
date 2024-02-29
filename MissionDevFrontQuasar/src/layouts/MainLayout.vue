<script setup lang="ts">
import { ref } from 'vue';
import { useUserStore } from 'src/stores/user-store';
import { useRouter } from 'vue-router';
// components
import SideBarMenu from 'src/components/layout/SideBarMenu.vue';
import ApplicationTitle from 'src/components/layout/ApplicationTitle.vue';

// consts
const userStore = useUserStore();
const router = useRouter();

// refs
const leftDrawer = ref(true);

// functions
function toggleLeftDrawer() {
  leftDrawer.value = !leftDrawer.value;
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
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
