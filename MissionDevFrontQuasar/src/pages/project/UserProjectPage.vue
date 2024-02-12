<script setup lang="ts">
import { api } from 'src/boot/axios';
// components
import ProjectTable from 'src/components/project/ProjectTable.vue';

// functions
async function getUserProjects(limit: number, offset: number) {
  return await api.get(`projects?offset=${offset}&limit=${limit}`);
}
</script>

<template>
  <q-page class="q-pa-lg">
    <h5 class="text-weight-bold no-margin q-pb-lg">Projets</h5>

    <ProjectTable :call-back-project="getUserProjects" />
  </q-page>
</template>
