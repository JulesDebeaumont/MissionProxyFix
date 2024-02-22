<script setup lang="ts">
import { useUserStore } from 'src/stores/user-store';
import { ref } from 'vue';
import useDragAndDrop from 'src/utils/drag-and-drop';
// vue-flow
import type { Elements } from '@vue-flow/core';
import { VueFlow } from '@vue-flow/core';
// components
import SketchSideBar from 'src/components/project/sketch/sidebar/SketchSideBar.vue';
import SketchCanvasGenericRecursiveComponent from 'src/components/project/sketch/canvas/SketchCanvasGenericRecursiveComponent.vue';

// props
const propsComponent = defineProps<{
  projectId: string;
}>();

// consts
const userStore = useUserStore();
const { onDragOver, onDrop, onDragLeave } = useDragAndDrop();

// refs
const elements = ref<Elements>([]);
</script>

<template>
  <div style="width: 40vw; height: 60vh" class="dndflow" @drop="onDrop">
    <VueFlow v-model="elements" @dragover="onDragOver" @dragleave="onDragLeave">
      <template #node-custom="element">
        <SketchCanvasGenericRecursiveComponent :componentData="element.data" />
      </template>
    </VueFlow>
    <SketchSideBar />
  </div>
</template>
