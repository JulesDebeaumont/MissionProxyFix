<script setup lang="ts">
import { useUserStore } from 'src/stores/user-store';
import { onMounted, onUnmounted, ref } from 'vue';
import { ISketchCore, IPosition } from 'src/components/models';
import useDragAndDrop from 'src/utils/drag-and-drop';
// vue-flow
import { NodePositionChange, VueFlow, useVueFlow } from '@vue-flow/core';
import '@vue-flow/core/dist/style.css';
import '@vue-flow/core/dist/theme-default.css';
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
const {
  panOnDrag,
  paneDragging,
  autoPanOnNodeDrag,
  panOnScroll,
  zoomOnScroll,
  onNodesChange,
  onNodeDrag,
  onNodeDragStop,
  applyNodeChanges,
} = useVueFlow();

onNodeDrag((nodeDragEvent) => {
  cursorX.value = nodeDragEvent.event.clientX;
  cursorY.value = nodeDragEvent.event.clientY;
  selectNodeId.value = nodeDragEvent.node.id;
});

onNodeDragStop((_nodeDragEvent) => {
  selectNodeId.value = null;
  xMode.value = false;
  yMode.value = false;
});

onNodesChange((changes) => {
  const positionChanges = changes.filter((change) => {
    return change.type === 'position' && change.dragging === true;
  }) as NodePositionChange[];
  if (positionChanges.length && selectNodeId.value !== null) {
    let nextXPosition = 0;
    let nextYPosition = 0;
    let stickyElementFound = false;

    if (stickyMode.value) {
      const currentHoveredElement = getNodeElementFromCursor(
        selectNodeId.value
      );
      if (currentHoveredElement !== null) {
        const rectElementPosition = getElementRectPositions(
          currentHoveredElement
        );
        const stickyPosition =
          getClosestRectPositionFromMouse(rectElementPosition);
        const stickyPositionForViewport =
          getElementPositionTranslatedForViewport(stickyPosition);
        nextXPosition = stickyPositionForViewport.x;
        nextYPosition = stickyPositionForViewport.y;
        stickyElementFound = true;
      }
    }

    if (xMode.value) {
      applyNodeChanges(
        positionChanges.map((change) => {
          if (stickyElementFound === false) {
            nextXPosition = change.position.x;
          }
          return {
            ...change,
            position: {
              y: change.from.y,
              x: nextXPosition,
            },
          };
        })
      );
    }
    if (yMode.value) {
      applyNodeChanges(
        positionChanges.map((change) => {
          if (stickyElementFound === false) {
            nextYPosition = change.position.y;
          }
          return {
            ...change,
            position: {
              y: nextYPosition,
              x: change.from.x,
            },
          };
        })
      );
    }
    if (
      yMode.value === false &&
      xMode.value === false &&
      stickyElementFound === true
    ) {
      applyNodeChanges(
        positionChanges.map((change) => {
          return {
            ...change,
            position: {
              x: nextXPosition,
              y: nextYPosition,
            },
          };
        })
      );
    }
  }
});

// refs
const elements = ref<ISketchCore[]>([]);
const xMode = ref(false);
const yMode = ref(false);
const cursorX = ref(0);
const cursorY = ref(0);
const stickyMode = ref(false);
const selectNodeId = ref<string | null>(null);
const slowMode = ref(false); // TODO

// functions

function pressKeyEvent(event: KeyboardEvent) {
  if (event.key === 'x') {
    xMode.value = true;
    yMode.value = false;
  }
  if (event.key === 'y') {
    yMode.value = true;
    xMode.value = false;
  }
  if (event.key === 'Control') {
    stickyMode.value = true;
  }
}
function releaseKeyEvent(event: KeyboardEvent) {
  if (event.key === 'Control') {
    stickyMode.value = false;
  }
}
function mouseMoveEvent(event: MouseEvent) {
  cursorX.value = event.clientX;
  cursorY.value = event.clientY;
}
function launchListeners() {
  document.addEventListener('keydown', pressKeyEvent);
  document.addEventListener('keyup', releaseKeyEvent);
  document.addEventListener('mousemove', mouseMoveEvent);
}
function removeListeners() {
  document.removeEventListener('keydown', pressKeyEvent);
  document.removeEventListener('keyup', pressKeyEvent);
  document.removeEventListener('mousemove', mouseMoveEvent);
}
function getNodeElementFromCursor(elementIdToAvoid: string) {
  const elements = document.elementsFromPoint(cursorX.value, cursorY.value);
  const nodeElementFound = elements.find((elementFind) => {
    return (
      elementFind.classList.contains('vue-flow__node') &&
      elementFind.getAttribute('data-id') !== elementIdToAvoid
    );
  });
  return nodeElementFound?.firstElementChild ?? null;
}
function getElementRectPositions(element: Element) {
  return element.getBoundingClientRect();
}
function getVueFlowViewportPosition() {
  const viewportContainer = document.getElementById(
    'mpf-sketch-viewport'
  ) as HTMLElement;
  return viewportContainer.getBoundingClientRect();
}
function getElementPositionTranslatedForViewport(elementPosition: IPosition) {
  const viewPortPosition = getVueFlowViewportPosition();
  return {
    x: elementPosition.x - viewPortPosition.left,
    y: elementPosition.y - viewPortPosition.top,
  };
}
function getClosestRectPositionFromMouse(rectPosition: DOMRect) {
  return [
    { x: rectPosition.left, y: rectPosition.top },
    { x: rectPosition.right, y: rectPosition.top },
    { x: rectPosition.left, y: rectPosition.bottom },
    { x: rectPosition.right, y: rectPosition.bottom },
  ]
    .sort((positionSortA, positionSortB) => {
      return (
        getDistanceBetweenPositionAndCursor(positionSortA) -
        getDistanceBetweenPositionAndCursor(positionSortB)
      );
    })
    .at(0) as IPosition;
}
function getDistanceBetweenPositionAndCursor(position: IPosition) {
  // Pythagore
  return Math.sqrt(
    Math.pow(position.x - cursorX.value, 2) +
      Math.pow(position.y - cursorY.value, 2)
  );
}

// lifeCycle
onMounted(() => {
  panOnDrag.value = false;
  paneDragging.value = false;
  autoPanOnNodeDrag.value = false;
  panOnScroll.value = false;
  zoomOnScroll.value = false;
  launchListeners();
});
onUnmounted(() => {
  removeListeners();
});
</script>

<template>
  xMode -> {{ xMode }} <br />
  yMode -> {{ yMode }} <br />
  Sticket -> {{ stickyMode }} <br />
  X Page -> {{ cursorX }} <br />
  Y PAge -> {{ cursorY }} <br />
  Node Selected {{ selectNodeId }}
  <SketchSideBar />
  <div
    style="width: 100vw; height: 60vh"
    class="dndflow bg-green column full-width"
    id="mpf-sketch-viewport"
    @drop="onDrop"
  >
    <VueFlow v-model="elements" @dragover="onDragOver" @dragleave="onDragLeave">
      <template #node-custom="element">
        <SketchCanvasCustomNode
          :componentData="element.data"
          :elementData="(({ ...element, data: undefined }) as ISketchCore)"
        />
      </template>
    </VueFlow>
  </div>
</template>
