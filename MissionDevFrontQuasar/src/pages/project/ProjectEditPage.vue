<script setup lang="ts">
import { useQuasar } from 'quasar';
import { api } from 'src/boot/axios';
import { IProject, IUser, IProjectUser } from 'src/components/models';
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { getAllStatesOptions } from 'src/utils/project';
import { dateFormatDDMMYYYY } from 'src/utils/datetime';

// consts
const route = useRoute();
const $q = useQuasar();
const defaultProject: Omit<IProject, 'id'> = {
  title: 'Nouveau projet',
  description: '',
  state: 0,
  deadline: dateFormatDDMMYYYY(new Date()),
  projectUsers: [],
};

// refs
const projectIdRef = ref(route.params.projectId as string);
const projectRef = ref<IProject | null>(null);
const isLoadingGet = ref(false);
const isLoadingSubmit = ref(false);
const userOptions = ref<IUser[]>([]);
const listOfUsersRef = ref<IUser[]>([]);

// functions
async function saveProject() {
  isLoadingSubmit.value = true;
  const projectPayload = {
    ...projectRef.value,
    projectUsers: listOfUsersRef.value.map((listOfUserMap) => {
      return {
        userId: listOfUserMap.id,
      } as IProjectUser;
    }),
  } as IProject;
  try {
    if (isEditMode.value) {
      await api.put(`Projects/${projectIdRef.value}`, projectPayload);
    } else {
      const responsePostProject = await api.post('Projects', projectPayload);
      projectRef.value = responsePostProject.data as IProject;
      projectIdRef.value = String(projectRef.value.id);
    }
    $q.notify({
      type: 'positive',
      message: 'PRoject mis à jour',
    });
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingSubmit.value = false;
  }
}
async function getProjectById(projectId: string) {
  isLoadingGet.value = true;
  try {
    const responseProject = await api.get(`Projects/${projectId}`);
    projectRef.value = responseProject.data as IProject;
    listOfUsersRef.value = responseProject.data.projectUsers as IUser[];
  } catch (error) {
    console.error(error);
  } finally {
    isLoadingGet.value = false;
  }
}
async function getUsersByFilter(
  val: string | null,
  update: (callback: () => void) => void,
  abort: () => void
) {
  if ((val?.length ?? 0) < 2) {
    abort();
    return;
  }
  setTimeout(() => {
    update(async () => {
      try {
        const needle = val?.toLocaleLowerCase();
        const responseUserOption = await api.get(
          `Users?fullnameSearch=${needle}`
        );
        userOptions.value = responseUserOption.data;
      } catch (error) {
        console.error(error);
      }
    });
  }, 0);
}
function disableOptionUser(userArg: IUser) {
  if (projectRef.value === null) {
    return false;
  }
  return (
    listOfUsersRef.value?.map((user) => user.id).includes(userArg.id) ?? false
  );
}

// computeds
const isEditMode = computed(() => {
  return projectIdRef.value !== undefined;
});

// lifeCycle
onMounted(async () => {
  if (isEditMode.value) {
    await getProjectById(projectIdRef.value);
  } else {
    projectRef.value = JSON.parse(JSON.stringify(defaultProject));
  }
});
</script>

<template>
  <q-page class="q-pa-lg">
    <h5 class="text-weight-bold no-margin q-pb-lg">
      {{ isEditMode ? 'Edition' : 'Création' }} de projet
    </h5>

    <q-card v-if="projectRef !== null" class="q-pa-md">
      <q-form @submit="saveProject" class="flex column">
        <div class="flex column q-gutter-y-sm">
          <q-input v-model="projectRef.title" label="Titre" filled />
          <q-input
            v-model="projectRef.description"
            label="Description"
            filled
          />
          <q-select
            v-model="projectRef.state"
            label="Etat"
            filled
            map-options
            emit-value
            :options="getAllStatesOptions()"
          />

          <q-input
            v-model="projectRef.deadline"
            filled
            label="Deadline"
            mask="####-##-##"
          >
            <template v-slot:append>
              <q-icon name="event" class="cursor-pointer">
                <q-popup-proxy
                  cover
                  transition-show="scale"
                  transition-hide="scale"
                >
                  <q-date v-model="projectRef.deadline" mask="YYYY-MM-DD">
                    <div class="row items-center justify-end">
                      <q-btn
                        v-close-popup
                        label="Fermer"
                        color="primary"
                        flat
                      />
                    </div>
                  </q-date>
                </q-popup-proxy>
              </q-icon>
            </template>
          </q-input>

          <q-select
            v-model="listOfUsersRef"
            label="Ajouter des membres"
            option-label="fullname"
            option-value="id"
            filled
            map-options
            multiple
            use-input
            clearable
            :option-disable="disableOptionUser"
            :options="userOptions"
            @filter="getUsersByFilter"
          >
            <template v-slot:selected-item="scope">
              <q-chip
                removable
                dense
                @remove="scope.removeAtIndex(scope.index)"
                :tabindex="scope.tabindex"
                color="info"
                text-color="white"
                class="q-ma-none q-pa-sm"
              >
                {{ scope.opt?.fullname }}
              </q-chip>
            </template>
          </q-select>
        </div>

        <div class="flex row no-wrap q-py-md">
          <q-btn type="submit" label="Enregistrer" no-caps color="primary" />
          <q-btn
            :to="{ name: 'myProjects' }"
            label="Retour"
            no-caps
            color="primary"
            class="q-ml-sm"
          />
        </div>
      </q-form>
    </q-card>
  </q-page>
</template>
