<script setup lang="ts">
import { onMounted } from 'vue';
import { useUserStore } from 'src/stores/user-store';
// components
import MainLayout from 'src/layouts/MainLayout.vue';

// lifeCycle
onMounted(() => {
  const userStore = useUserStore();
  if (userStore.jwtInCookie) {
    userStore.setupUserFromJwtInCookies();
  }
});
</script>

<template>
  <main-layout />
</template>
