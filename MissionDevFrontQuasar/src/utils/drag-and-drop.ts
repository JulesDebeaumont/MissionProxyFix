import { useVueFlow } from '@vue-flow/core';
import { ref, watch } from 'vue';
import { ISidebarComponentData } from './sketch-sidebar';

let id = 0;

function getId() {
  return `dndnode_${id++}`;
}

const state = {
  draggedComponentData: ref<ISidebarComponentData | null>(null),
  isDragOver: ref(false),
  isDragging: ref(false),
};

export default function useDragAndDrop() {
  const { isDragOver, isDragging, draggedComponentData } = state;
  const { addNodes, screenToFlowCoordinate, onNodesInitialized, updateNode } =
    useVueFlow();

  watch(isDragging, (dragging) => {
    document.body.style.userSelect = dragging ? 'none' : '';
  });

  function onDragStart(event: DragEvent, componentData: ISidebarComponentData) {
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
    isDragging.value = true;
    draggedComponentData.value = componentData;

    document.addEventListener('drop', onDragEnd);
  }

  function onDragOver(event: DragEvent) {
    event.preventDefault();

    isDragOver.value = true;

    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  function onDragLeave() {
    isDragOver.value = false;
  }

  function onDragEnd() {
    isDragging.value = false;
    isDragOver.value = false;
    document.removeEventListener('drop', onDragEnd);
  }

  function onDrop(event: DragEvent) {
    const position = screenToFlowCoordinate({
      x: event.clientX,
      y: event.clientY,
    });

    const nodeId = getId();

    const newNode = {
      id: nodeId,
      type: 'custom',
      data: draggedComponentData.value,
      position,
    };

    const { off } = onNodesInitialized(() => {
      updateNode(nodeId, (node) => ({
        position: {
          x: node.position.x - node.dimensions.width / 2,
          y: node.position.y - node.dimensions.height / 2,
        },
      }));

      off();
    });

    addNodes(newNode);
  }

  return {
    isDragOver,
    isDragging,
    onDragStart,
    onDragLeave,
    onDragOver,
    onDrop,
  };
}
