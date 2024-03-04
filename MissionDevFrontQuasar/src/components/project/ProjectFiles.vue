<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { IProjectFile } from 'src/components/models';
import { api } from 'src/boot/axios';
import { exportFile } from 'quasar';
import { getImgByFileExtension } from 'src/utils/file';
import { dateFormatDDMMYYYYHHMM } from 'src/utils/datetime';
// components
import DelayedCard from 'src/components/global/DelayedCard.vue';

// props
const propsComponent = defineProps<{
  projectId: string;
}>();

// refs
const projectFiles = ref<IProjectFile[] | null>(null);
const isLoadingGet = ref(false);
const isLoadingDownload = ref(false);
const isLoadingUpload = ref(false);
const dialogUpload = ref(false);
const dialogRemove = ref(false);
const uploadFileRef = ref();
const removeProjectFileData = ref<IProjectFile | null>(null);

// functions
async function getProjectFiles() {
  isLoadingGet.value = true;
  try {
    const responseGetFiles = await api.get(
      `projectFiles/Projects/${propsComponent.projectId}`
    );
    projectFiles.value = responseGetFiles.data as IProjectFile[];
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingGet.value = false;
  }
}
async function downloadProjectFile(projectFile: IProjectFile) {
  isLoadingDownload.value = true;
  try {
    const responseDownload = await api.get(
      `ProjectFiles/Download/${projectFile.id}`,
      { responseType: 'blob' }
    );
    exportFile(projectFile.filename, responseDownload.data);
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingDownload.value = false;
  }
}
async function uploadProjectFiles() {
  isLoadingUpload.value = true;
  try {
    const formData = new FormData();
    formData.append('files', uploadFileRef.value);
    formData.append('ProjectId', propsComponent.projectId);
    await api.post('projectFiles/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    await getProjectFiles();
  } catch (error: any) {
    console.error(error);
  } finally {
    isLoadingUpload.value = false;
    dialogUpload.value = false;
    uploadFileRef.value = undefined;
  }
}
async function removeProjectFile(projectFile: IProjectFile) {
  if (projectFiles.value === null) {
    return;
  }
  try {
    await api.delete(`projectFiles/Delete/${projectFile.id}`);
    projectFiles.value = projectFiles.value.filter((projectFileFilter) => {
      return projectFileFilter.id !== projectFile.id;
    });
  } catch (error: any) {
    console.error(error);
  } finally {
    removeProjectFileData.value = null;
    dialogRemove.value = false;
  }
}
async function toggleIsSharedProjectFile(projectFile: IProjectFile) {
  const responseToggleFile = await api.put(
    `projectFiles/ToggleIsShared/${projectFile.id}`
  );
  projectFile.isShared = responseToggleFile.data.isShared as boolean;
}
function setupDeleteFile(projectFile: IProjectFile) {
  removeProjectFileData.value = projectFile;
  dialogRemove.value = true;
}

// lifeCycle
onMounted(async () => {
  await getProjectFiles();
});
</script>

<template>
  <q-dialog v-model="dialogUpload">
    <q-card class="q-pa-md">
      <q-card-section>
        <h6 class="no-margin text-center">Upload de fichier</h6>

        <div class="q-py-md flex column q-gutter-y-md">
          <q-file clearable filled v-model="uploadFileRef" label="Fichier" />

          <q-btn @click="uploadProjectFiles" color="secondary" label="Upload" />
        </div>
      </q-card-section>
    </q-card>
  </q-dialog>

  <q-dialog v-model="dialogRemove">
    <DelayedCard
      v-if="removeProjectFileData !== null"
      message="Voulez-vous vraiment supprimer ce fichier ?"
      label-button-action="Supprimer"
      :description="removeProjectFileData.filename"
      :delay-in-second="1"
      @button-action="removeProjectFile(removeProjectFileData)"
    />
  </q-dialog>

  <div v-if="isLoadingGet === false && projectFiles !== null">
    <q-list v-if="projectFiles.length > 0" bordered separator>
      <q-item
        v-for="projectFile in projectFiles"
        :key="projectFile.id"
        clickable
      >
        <q-item-section avatar>
          <img
            :src="`/images/${getImgByFileExtension(
              projectFile.filename.split('.').at(-1) ?? ''
            )}`"
            width="48"
            height="48"
          />
        </q-item-section>

        <q-item-section>
          <q-item-label>{{ projectFile.filename }}</q-item-label>
          <q-item-label caption
            >{{ dateFormatDDMMYYYYHHMM(projectFile.createdAt) }} par
            {{ projectFile.user?.fullname }}</q-item-label
          >
        </q-item-section>

        <q-item-section side>
          <div class="flex row no-wrap q-gutter-x-sm">
            <q-btn
              round
              icon="download"
              color="info"
              size="sm"
              @click="downloadProjectFile(projectFile)"
            >
              <q-tooltip>Télécharger</q-tooltip>
            </q-btn>

            <q-btn
              round
              :icon="projectFile.isShared ? 'share' : 'lock'"
              color="warning"
              size="sm"
              @click="toggleIsSharedProjectFile(projectFile)"
            >
              <q-tooltip
                >{{ projectFile.isShared ? 'Désactiver' : 'Activer' }} le
                partage</q-tooltip
              >
            </q-btn>

            <q-btn
              round
              icon="delete"
              color="negative"
              size="sm"
              @click="setupDeleteFile(projectFile)"
            >
              <q-tooltip>Supprimer</q-tooltip>
            </q-btn>
          </div>
        </q-item-section>
      </q-item>
    </q-list>

    <div v-else>Aucun document</div>
  </div>

  <q-btn
    @click="dialogUpload = true"
    label="Upload"
    color="primary"
    class="q-mt-md"
  />

  <div v-show="isLoadingGet" class="flex">
    <q-spinner color="primary" size="md" />
  </div>
</template>
