<script setup lang="ts">
import { toRef, watch } from 'vue';
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
import { ISketchCore } from 'src/components/models';
// components
import SketchCanvasGenericRecursiveComponent from 'src/components/project/sketch/canvas/SketchCanvasGenericRecursiveComponent.vue';

// props
const propsComponent = defineProps<{
  componentData: ISidebarComponentData;
  elementData: ISketchCore;
}>();

// refs
const elementDataRef = toRef(propsComponent.elementData);
</script>

<template>
  <SketchCanvasGenericRecursiveComponent
    :component-data="componentData"
    :is-root="true"
  />
</template>
