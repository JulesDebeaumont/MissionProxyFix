<script setup lang="ts">
import { api } from 'src/boot/axios';
import { IProject } from 'src/components/models';
import { onMounted, ref } from 'vue';

// refs
const isLoading = ref(false);
const projects = ref<IProject[] | null>(null);

// functions
async function getProjects() {
  isLoading.value = true;
  const responseProjects = await api.get('projects');
  projects.value = responseProjects.data;
  isLoading.value = false;
}

// lifeCycle
onMounted(async () => {
  await getProjects();
});
</script>

<template>
  <q-page class="row items-center justify-evenly">
    {{ projects }}
  </q-page>
</template>
