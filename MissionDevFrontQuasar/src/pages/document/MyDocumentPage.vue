<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { IUserFile } from 'src/components/models';
import { api } from 'src/boot/axios';
import { exportFile } from 'quasar';
import { getImgByFileExtension } from 'src/utils/file';
import { dateFormatDDMMYYYYHHMM } from 'src/utils/datetime';
// components
import DelayedCardVue from 'src/components/global/DelayedCard.vue';

// refs
const userFiles = ref<IUserFile[] | null>(null);
const isLoadingGet = ref(false);
const isLoadingDownload = ref(false);
const isLoadingUpload = ref(false);
const dialogUpload = ref(false);
const dialogRemove = ref(false);
const uploadFileRef = ref();
const removeUserFileData = ref<IUserFile | null>(null);

// functions
async function getUserFiles() {
  isLoadingGet.value = true;
  try {
    const responseGetFiles = await api.get('userFiles');
    userFiles.value = responseGetFiles.data as IUserFile[];
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingGet.value = false;
  }
}
async function downloadUserFile(userFile: IUserFile) {
  isLoadingDownload.value = true;
  try {
    const responseDownload = await api.get(
      `userFiles/Download/${userFile.id}`,
      { responseType: 'blob' }
    );
    exportFile(userFile.filename, responseDownload.data);
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingDownload.value = false;
  }
}
async function uploadUserFiles() {
  isLoadingUpload.value = true;
  try {
    const formData = new FormData();
    formData.append('files', uploadFileRef.value);
    await api.post('userFiles/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    await getUserFiles();
  } catch (error: any) {
    console.error(error);
  } finally {
    isLoadingUpload.value = false;
    dialogUpload.value = false;
    uploadFileRef.value = undefined;
  }
}
async function removeUserFile(userFile: IUserFile) {
  if (userFiles.value === null) {
    return;
  }
  try {
    await api.delete(`userFiles/Delete/${userFile.id}`);
    userFiles.value = userFiles.value.filter((userFileFilter) => {
      return userFileFilter.id !== userFile.id;
    });
  } catch (error: any) {
    console.error(error);
  } finally {
    removeUserFileData.value = null;
    dialogRemove.value = false;
  }
}
function setupDeleteFile(userFile: IUserFile) {
  removeUserFileData.value = userFile;
  dialogRemove.value = true;
}

// lifeCycle
onMounted(async () => {
  await getUserFiles();
});
</script>

<template>
  <q-page class="q-pa-lg">
    <q-dialog v-model="dialogUpload">
      <q-card class="q-pa-md">
        <q-card-section>
          <h6 class="no-margin text-center">Upload de fichier</h6>

          <div class="q-py-md flex column q-gutter-y-md">
            <q-file clearable filled v-model="uploadFileRef" label="Fichier" />

            <q-btn @click="uploadUserFiles" color="secondary" label="Upload" />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>

    <q-dialog v-model="dialogRemove">
      <DelayedCardVue
        v-if="removeUserFileData !== null"
        message="Voulez-vous vraiment supprimer ce fichier ?"
        label-button-action="Supprimer"
        :description="removeUserFileData.filename"
        :delay-in-second="1"
        @button-action="removeUserFile(removeUserFileData)"
      />
    </q-dialog>

    <h5 class="text-weight-bold no-margin q-pb-lg">Mes documents</h5>

    <div v-if="isLoadingGet === false && userFiles !== null">
      <q-list v-if="userFiles.length > 0" bordered separator>
        <q-item v-for="userFile in userFiles" :key="userFile.id" clickable>
          <q-item-section avatar>
            <img
              :src="`/images/${getImgByFileExtension(
                userFile.filename.split('.').at(-1) ?? ''
              )}`"
              width="48"
              height="48"
            />
          </q-item-section>

          <q-item-section>
            <q-item-label>{{ userFile.filename }}</q-item-label>
            <q-item-label caption>{{
              dateFormatDDMMYYYYHHMM(userFile.createdAt)
            }}</q-item-label>
          </q-item-section>

          <q-item-section side>
            <div class="flex row no-wrap q-gutter-x-sm">
              <q-btn
                round
                icon="download"
                color="info"
                size="sm"
                @click="downloadUserFile(userFile)"
              >
                <q-tooltip>Télécharger</q-tooltip>
              </q-btn>

              <q-btn
                round
                icon="delete"
                color="negative"
                size="sm"
                @click="setupDeleteFile(userFile)"
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
  </q-page>
</template>
