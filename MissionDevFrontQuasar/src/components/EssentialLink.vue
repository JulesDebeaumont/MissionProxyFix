<script setup lang="ts">
import { IProject } from './models';

// props
const propsComponents = defineProps<{
  project: IProject;
}>()
</script>


<template>
  <q-item
    clickable
    tag="a"
    target="_blank"
    :to="{ name: '', params: { projectId: propsComponents.project.ID }}"
  >
    <q-item-section
      avatar
    >
      <q-icon name="thunder" />
    </q-item-section>

    <q-item-section>
      <q-item-label>{{ propsComponents.project.Title }}</q-item-label>
    </q-item-section>
  </q-item>
</template>

