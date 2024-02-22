<script setup lang="ts">
import { api } from 'src/boot/axios';
import { IProject } from 'src/components/models';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
// components
import ZimbraTab from 'src/components/project/ZimbraTab.vue';
import SketchTab from 'src/components/project/sketch/SketchTab.vue';

// consts
const route = useRoute();
const projectId = route.params.projectId as string;

// refs
const projectRef = ref<IProject | null>(null);
const isLoading = ref(false);

// functions
async function getProject() {
  isLoading.value = true;
  const responseProject = await api.get(`projects/${projectId}`);
  projectRef.value = responseProject.data as IProject;
  isLoading.value = false;
}

// lifeCycle
onMounted(async () => {
  await getProject();
});
</script>

<template>
  <div>
    <div>
      <pre>{{ projectRef }}</pre>
      <ZimbraTab :project-id="projectId" />
      <SketchTab :project-id="projectId" />
    </div>

    <div v-show="isLoading" class="flex flex-center">
      <q-spinner color="primary" />
    </div>
  </div>
</template>
