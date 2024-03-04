<script setup lang="ts">
import { api } from 'src/boot/axios';
import { IProject } from 'src/components/models';
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { QTabProps } from 'quasar';
import { dateFormatDDMMYYYY } from 'src/utils/datetime';
import {
  displayProjectState,
  getColorClassByProjectState,
} from 'src/utils/project';
// components
import ZimbraTab from 'src/components/project/ZimbraTab.vue';
import SketchTab from 'src/components/project/sketch/SketchTab.vue';
import ProjectFiles from 'src/components/project/ProjectFiles.vue';
import DelayedCard from 'src/components/global/DelayedCard.vue';

// consts
const route = useRoute();
const router = useRouter();
const projectId = route.params.projectId as string;
const applicationTab: QTabProps[] = [
  {
    name: 'files',
    icon: 'folder',
    label: 'Fichiers',
  },
  {
    name: 'mail',
    icon: 'mail',
    label: 'Mail',
  },
  {
    name: 'sketches',
    icon: 'gesture',
    label: 'Sketches',
  },
];

// refs
const projectRef = ref<IProject | null>(null);
const isLoading = ref(false);
const dialogDeleteProject = ref(false);
const tab = ref('');

// functions
async function getProject() {
  isLoading.value = true;
  const responseProject = await api.get(`projects/${projectId}`);
  projectRef.value = responseProject.data as IProject;
  isLoading.value = false;
}
async function deleteProject() {
  await api.delete(`projects/${projectId}`);
  router.push({ name: 'root' });
}

// lifeCycle
onMounted(async () => {
  await getProject();
  tab.value = String(applicationTab[0].name);
});
</script>

<template>
  <q-page class="q-pa-lg">
    <q-dialog v-model="dialogDeleteProject">
      <DelayedCard
        message="Voulez-vous vraiment supprimer ce projet ?"
        @button-action="deleteProject"
        :delayInSecond="2"
        labelButtonAction="Supprimer"
      />
    </q-dialog>

    <div
      v-if="isLoading === false"
      class="q-pb-xl q-mb-xl flex column full-width"
    >
      <div
        v-if="projectRef !== null"
        class="flex flex-center column full-width"
      >
        <q-intersection once transition="slide-down" class="full-width q-px-xl">
          <div class="flex row no-wrap items-center">
            <h4 class="text-center text-weight-bold q-mb-xs q-mt-md">
              {{ projectRef.title }}
            </h4>
            <div class="flex row no-wrap q-pt-md q-pl-md">
              <q-icon
                @click="
                  router.push({
                    name: 'projectEdit',
                    params: { projectId },
                  })
                "
                color="warning"
                name="edit"
                class="cursor-pointer"
                size="sm"
              >
                <q-tooltip>Editer</q-tooltip>
              </q-icon>
              <q-icon
                @click="dialogDeleteProject = true"
                color="negative"
                name="delete"
                class="cursor-pointer"
                size="sm"
              >
                <q-tooltip>Supprimer</q-tooltip>
              </q-icon>
            </div>
          </div>

          <div class="flex column items-start full-width">
            <div class="flex items-start column text-body1 q-py-md">
              <span>
                <span class="text-weight-medium">Description :</span>
                {{ projectRef.description }}
              </span>
              <span>
                <span class="text-weight-medium">Etat : </span>
                <q-chip
                  :color="getColorClassByProjectState(projectRef)"
                  text-color="white"
                  size="sm"
                >
                  {{ displayProjectState(projectRef) }}
                </q-chip>
              </span>
              <span>
                <span class="text-weight-medium">Deadline :</span>
                {{
                  projectRef.deadline
                    ? dateFormatDDMMYYYY(projectRef.deadline)
                    : '???'
                }}
              </span>
              <span
                ><span class="text-weight-medium">Utilisateurs :</span>
                <q-chip
                  v-for="user in projectRef.projectUsers"
                  :key="user.id"
                  color="info"
                  text-color="white"
                  size="sm"
                >
                  {{ user.fullname }}
                </q-chip>
              </span>
            </div>
          </div>
        </q-intersection>

        <q-intersection
          once
          transition="slide-up"
          class="full-width q-px-xl q-mt-md"
        >
          <q-tabs
            v-model="tab"
            inline-label
            class="bg-secondary text-white rounded-borders"
            active-bg-color="primary"
            style="width: 100%"
          >
            <q-tab
              v-for="tabInfo in applicationTab"
              :key="tabInfo.name"
              :name="tabInfo.name"
              :icon="tabInfo.icon"
              :label="tabInfo.label"
              class="full-width"
            />
          </q-tabs>

          <q-tab-panels v-model="tab" animated class="shadow-2">
            <q-tab-panel :name="applicationTab[0].name">
              <ProjectFiles :project-id="projectId" />
            </q-tab-panel>

            <q-tab-panel :name="applicationTab[1].name">
              <ZimbraTab :project-id="projectId" />
            </q-tab-panel>

            <q-tab-panel :name="applicationTab[2].name">
              <SketchTab :project-id="projectId" />
            </q-tab-panel>
          </q-tab-panels>
        </q-intersection>
      </div>
    </div>

    <div v-show="isLoading" class="flex flex-center">
      <q-spinner color="primary" />
    </div>
  </q-page>
</template>
