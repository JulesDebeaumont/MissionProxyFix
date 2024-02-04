<script setup lang="ts">
import { useUserStore } from 'src/stores/user-store';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

// consts
const userStore = useUserStore();
const router = useRouter();

// refs
const username = ref<string | null>('didier');
const passwordLoginForm = ref<string | null>('_SuperDidier1234_');

// function
async function login() {
  if (username.value !== null && passwordLoginForm.value !== null) {
    await userStore.getJwt(username.value, passwordLoginForm.value);
    router.push({ name: 'dashboard' });
  }
}
</script>

<template>
  <q-page class="row items-center justify-evenly">
    <q-form @submit="login()">
      <q-input v-model="username" label="Pseudo" />
      <q-input v-model="passwordLoginForm" label="Mot de passe" />

      <q-btn label="Connexion" type="submit" />
    </q-form>
  </q-page>
</template>
