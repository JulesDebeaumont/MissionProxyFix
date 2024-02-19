<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { api } from 'src/boot/axios';
import { IProject } from 'src/components/models';
import { QTableProps, useQuasar } from 'quasar';
import { useRouter } from 'vue-router';
import { dateFormatDDMMYYYY } from 'src/utils/datetime';
import {
  displayProjectState,
  getColorClassByProjectState,
} from 'src/utils/project';
// components
import DelayedCard from 'src/components/global/DelayedCard.vue';

// props
const propsComponents = defineProps<{
  callBackProject: (limit: number, offset: number) => Promise<any>;
}>();

// consts
const projectTableColumns: QTableProps['columns'] = [
  {
    name: 'title',
    align: 'left',
    label: 'Titre',
    field: 'title',
    required: true,
    sortable: true,
  },
  {
    name: 'state',
    align: 'left',
    label: 'Etat',
    field: 'state',
    sortable: true,
  },
  {
    name: 'deadline',
    align: 'left',
    label: 'Deadline',
    field: 'deadline',
    sortable: true,
  },
  {
    name: 'users',
    align: 'left',
    label: 'Devs',
    field: 'users',
  },
  {
    name: 'actions',
    align: 'left',
    label: 'Actions',
    field: 'actions',
  },
];
const $q = useQuasar();
const router = useRouter();

// refs
const dialogDelete = ref(false);
const dialogDeleteConfig = ref<IProject | null>(null);
const isLoading = ref(false);
const projects = ref<IProject[]>([]);
const tableProjects = ref();
const filterByNameRef = ref<string | null>(null);
const pagination = ref({
  descending: false,
  page: 1,
  rowsPerPage: 20,
  rowsNumber: 0,
});

// functions
function setupDialogDeleteProject(project: IProject) {
  dialogDeleteConfig.value = project;
  dialogDelete.value = true;
}
async function filterProjectByName(nameFilter: string | number | null = null) {
  const euuuh = 0; // TODO
}
async function deleteProject() {
  await api.delete(`projects/${dialogDeleteConfig.value?.id}`);
  projects.value =
    projects.value?.filter(
      (project) => project.id !== dialogDeleteConfig.value?.id
    ) ?? [];
  dialogDelete.value = false;
  dialogDeleteConfig.value = null;
  $q.notify({
    type: 'positive',
    message: 'Projet supprimé',
  });
}
async function getProjectsQTable(propsArg: any) {
  isLoading.value = true;
  const { page, rowsPerPage } = propsArg.pagination;
  const fetchCount =
    rowsPerPage === 0 ? pagination.value.rowsNumber : rowsPerPage;
  const startRow = (page - 1) * rowsPerPage;
  const responseProject = await propsComponents.callBackProject(
    fetchCount,
    startRow
  );
  projects.value = responseProject.data?.projects;
  pagination.value.rowsNumber = responseProject.data?.rowCount;
  pagination.value.page = page;
  pagination.value.rowsPerPage = rowsPerPage;
  isLoading.value = false;
}

// lifeCycle
onMounted(async () => {
  await tableProjects.value.requestServerInteraction();
});
</script>

<template>
  <div class="full-width full-height">
    <q-dialog v-model="dialogDelete">
      <DelayedCard
        v-if="dialogDeleteConfig !== null"
        message="Voulez-vous vraiment supprimer ce projet ?"
        @button-action="deleteProject"
        :delayInSecond="3"
        labelButtonAction="Supprimer"
      />
    </q-dialog>

    <q-table
      :rows="projects"
      :columns="projectTableColumns"
      ref="tableProjects"
      v-model:pagination="pagination"
      row-key="id"
      no-data-label="Aucune donnée 🥕"
      no-results-label="Aucun résultat 🥕"
      class="bg-grey-2"
      binary-state-sort
      @request="getProjectsQTable"
    >
      <template #top-right>
        <q-btn
          @click="router.push({ name: 'project-new' })"
          color="primary"
          label="Créer un projet"
          class="q-mb-lg"
          no-caps
        />
      </template>

      <template #top-left>
        <q-input
          v-model="filterByNameRef"
          borderless
          dense
          placeholder="Filtrer par nom..."
          debounce="400"
          class="bg-white"
          clearable
          rounded
          @update:model-value="filterProjectByName"
        >
          <template #prepend>
            <q-icon name="search" class="q-ml-sm" />
          </template>
        </q-input>
      </template>

      <template v-slot:body="props">
        <q-tr :props="props" class="bg-white q-my-xs">
          <q-td key="title" :props="props">
            {{ props.row.title }}
          </q-td>
          <q-td key="state">
            <q-chip
              :color="getColorClassByProjectState(props.row)"
              text-color="white"
              size="sm"
            >
              {{ displayProjectState(props.row) }}
            </q-chip>
          </q-td>
          <q-td key="deadline">
            {{
              props.row.deadline
                ? dateFormatDDMMYYYY(props.row.deadline)
                : '???'
            }}
          </q-td>
          <q-td key="users">
            <q-chip
              v-for="user in props.row.users"
              :key="user.id"
              color="info"
              text-color="white"
              size="sm"
            >
              {{ user.fullname }}
            </q-chip>
          </q-td>
          <q-td key="actions" :props="props">
            <q-icon
              @click="
                router.push({
                  name: `project-show`,
                  params: { projectId: props.row.id },
                })
              "
              name="visibility"
              color="primary"
              size="sm"
              class="cursor-pointer"
            >
              <q-tooltip>Voir</q-tooltip>
            </q-icon>
            <q-icon
              @click="
                router.push({
                  name: 'project-edit',
                  params: { projectId: props.row.id },
                })
              "
              name="edit"
              color="warning"
              size="sm"
              class="cursor-pointer"
            >
              <q-tooltip>Editer</q-tooltip>
            </q-icon>
            <q-icon
              @click="setupDialogDeleteProject(props.row)"
              name="delete"
              size="sm"
              color="negative"
              class="cursor-pointer"
            >
              <q-tooltip>Supprimer</q-tooltip>
            </q-icon>
          </q-td>
        </q-tr>
      </template>
    </q-table>
  </div>
</template>

<style scoped>
td {
  height: 65px !important;
}
</style>
