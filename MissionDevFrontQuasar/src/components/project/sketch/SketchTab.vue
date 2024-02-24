<script setup lang="ts">
import { useUserStore } from 'src/stores/user-store';
import { onMounted, onUnmounted, ref } from 'vue';
import useDragAndDrop from 'src/utils/drag-and-drop';
// vue-flow
import type { Elements } from '@vue-flow/core';
import { VueFlow, useVueFlow } from '@vue-flow/core';
// components
import SketchSideBar from 'src/components/project/sketch/sidebar/SketchSideBar.vue';
import SketchCanvasCustomNode from 'src/components/project/sketch/canvas/SketchCanvasCustomNode.vue';

// props
const propsComponent = defineProps<{
  projectId: string;
}>();

// consts
const userStore = useUserStore();
const { onDragOver, onDrop, onDragLeave } = useDragAndDrop();
const { panOnDrag, nodesDraggable } = useVueFlow();

// refs
const elements = ref<Elements>([]);
const grabMode = ref(false);
const xMode = ref(false);
const yMode = ref(false);

// functions

function pressKeyEvent(event: KeyboardEvent) {
  if (event.key === 'g') {
    grabMode.value = true;
  }
  if (event.key === 'x' && grabMode.value === true) {
    xMode.value = true;
    yMode.value = false;
  }
  if (event.key === 'y' && grabMode.value === true) {
    yMode.value = true;
    xMode.value = false;
  }
  if (event.key === 'Enter' && grabMode.value === true) {
    grabMode.value = false;
    xMode.value = false;
    yMode.value = false;
  }
}
function launchListeners() {
  document.addEventListener('keydown', pressKeyEvent);
}
function removeListeners() {
  document.removeEventListener('keydown', pressKeyEvent);
}

// lifeCycle
onMounted(() => {
  panOnDrag.value = false;
  nodesDraggable.value = false;
  launchListeners();
});
onUnmounted(() => {
  removeListeners();
});
</script>

<template>
  Grabmode -> {{ grabMode }} <br />
  xMode -> {{ xMode }} <br />
  yMode -> {{ yMode }}
  <div style="width: 40vw; height: 60vh" class="dndflow" @drop="onDrop">
    <VueFlow v-model="elements" @dragover="onDragOver" @dragleave="onDragLeave">
      <template #node-custom="element">
        <SketchCanvasCustomNode
          :componentData="element.data"
          :elementData="{ ...element, data: undefined }"
          :isSelected="element.selected"
          :grabMode="grabMode"
          :xMode="xMode"
          :yMode="yMode"
        />
      </template>
    </VueFlow>
    <SketchSideBar />
  </div>
</template>
