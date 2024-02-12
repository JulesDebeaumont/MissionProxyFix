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
  <q-page>
    <div class="flex flex-center column q-pt-xl">
      <h6 class="no-margin q-py-md">
        Vous devez être connecté pour accéder aux fonctionnalités de
        <i>Mission Proxy Fix</i>
      </h6>

      <div class="flex flex-center column q-gutter-y-sm q-py-md">
        <q-input v-model="username" filled label="Email" />
        <q-input v-model="passwordLoginForm" filled label="Mot de passe" />
      </div>

      <q-btn color="primary" label="Connexion" @click="login" />
    </div>
  </q-page>
</template>
