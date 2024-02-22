<script setup lang="ts">
import useDragAndDrop from 'src/utils/drag-and-drop';
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
// components
import SketchSidebarGenericRecursiveComponent from 'src/components/project/sketch/sidebar/SketchSidebarGenericRecursiveComponent.vue';

// props
const propsComponent = defineProps<{
  listOfComponent: ISidebarComponentData[];
}>();

// consts
const { onDragStart } = useDragAndDrop();
</script>

<template>
  <div
    v-for="(chipCompo, index) in propsComponent.listOfComponent"
    :key="index"
    class="q-gutter-sm"
  >
    <SketchSidebarGenericRecursiveComponent
      :componentData="chipCompo"
      :draggable="true"
      @dragstart="onDragStart($event, chipCompo)"
    />
  </div>
</template>
