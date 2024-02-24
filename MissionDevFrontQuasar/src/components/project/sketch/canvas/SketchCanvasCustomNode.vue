<script setup lang="ts">
import { toRef, watch } from 'vue';
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
// components
import SketchCanvasGenericRecursiveComponent from 'src/components/project/sketch/canvas/SketchCanvasGenericRecursiveComponent.vue';

// props
const propsComponent = defineProps<{
  componentData: ISidebarComponentData;
  elementData: any;
  isSelected: boolean;
  grabMode: boolean;
  xMode: boolean;
  yMode: boolean;
}>();

// refs
const elementDataRef = toRef(propsComponent.elementData);

// watchs
watch(
  () => propsComponent.grabMode,
  (newValue, oldValue) => {
    if (propsComponent.isSelected && propsComponent.grabMode) {
      elementDataRef.value.draggable = true;
      elementDataRef.value.dragging = true;
    }
    if (
      propsComponent.grabMode === false ||
      propsComponent.isSelected === false
    ) {
      elementDataRef.value.draggable = false;
      elementDataRef.value.dragging = false;
    }
  }
);
</script>

<template>
  <SketchCanvasGenericRecursiveComponent
    :component-data="componentData"
    :is-root="true"
  />
</template>
