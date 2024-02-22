<script setup lang="ts">
import {
  ISidebarComponentData,
  buttonComponents,
  cardComponents,
  chipComponents,
} from 'src/utils/sketch-sidebar';
// components
import SketchSideBarMenu from 'src/components/project/sketch/sidebar/SketchSideBarMenu.vue';

// interfaces
interface ISketchSidebarMenu {
  label: string;
  listOfComponent: ISidebarComponentData[];
}

// consts
const allSideBarMenu: ISketchSidebarMenu[] = [
  {
    label: 'Boutton',
    listOfComponent: buttonComponents,
  },
  {
    label: 'Chip',
    listOfComponent: chipComponents,
  },
  {
    label: 'Card',
    listOfComponent: cardComponents,
  },
];
</script>

<template>
  <div>
    <q-expansion-item
      v-for="menu in allSideBarMenu"
      :key="menu.label"
      expand-separator
      :label="menu.label"
    >
      <SketchSideBarMenu :listOfComponent="menu.listOfComponent" />
    </q-expansion-item>
  </div>
</template>
