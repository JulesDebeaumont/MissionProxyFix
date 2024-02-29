<script setup lang="ts">
import { useRoute } from 'vue-router';

// interfaces
interface ISidebarMenu {
  label: string;
  routeName: string;
  routeCheckString: string;
  icon: string;
}

// consts
const route = useRoute();
const menus: ISidebarMenu[] = [
  {
    label: 'Projets',
    routeName: 'myProjects',
    routeCheckString: 'project',
    icon: 'rocket_launch',
  },
  {
    label: 'Documents',
    routeName: 'documents',
    routeCheckString: 'document',
    icon: 'description',
  },
];

// functions
function isPartOfRoute(menu: ISidebarMenu) {
  return route.name?.toString().toLowerCase().includes(menu.routeCheckString);
}
</script>

<template>
  <q-list class="q-px-xs flex flex-center" separator>
    <q-item
      class="q-py-sm full-width text-white"
      clickable
      v-for="menu in menus"
      :key="menu.label"
      :to="{ name: menu.routeName }"
    >
      <q-item-section avatar>
        <q-item-section avatar>
          <q-avatar
            :color="isPartOfRoute(menu) ? 'primary' : 'grey-7'"
            :text-color="isPartOfRoute(menu) ? 'white' : 'grey-3'"
            :icon="menu.icon"
          />
        </q-item-section>
      </q-item-section>

      <q-item-section>{{ menu.label }}</q-item-section>
    </q-item>
  </q-list>
</template>
