<script setup lang="ts">
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
import { ref } from 'vue';
// components
import SketchCanvasGenericRecursiveComponent from 'src/components/project/sketch/canvas/SketchCanvasGenericRecursiveComponent.vue';
import SketchComponentMenu from 'src/components/project/sketch/canvas/SketchComponentMenu.vue';
import SketchComponentRawContentEditor from 'src/components/project/sketch/canvas/SketchComponentRawContentEditor.vue';

// props
const propsComponent = defineProps<{
  componentData: ISidebarComponentData;
}>();

// refs
const elementDataRef = ref(propsComponent.componentData);

// functions
function updateComponentRef(elementDataArg: ISidebarComponentData) {
  elementDataRef.value = elementDataArg;
}
function updateComponentRawContent(rawContent: string) {
  elementDataRef.value.rawContent = rawContent;
}
function copyToClipBoard(propsAsString: string, content: string) {
  navigator.clipboard.writeText(`TODO ${propsAsString} > ${content} TODO`);
}
</script>

<template>
  <Component v-bind="elementDataRef.props" :is="elementDataRef.type">
    <template v-if="elementDataRef.rawContent !== undefined">
      {{ elementDataRef.rawContent }}
      <SketchComponentRawContentEditor
        :rawContent="elementDataRef.rawContent"
        @update-component-raw-content="updateComponentRawContent"
      />
    </template>
    <template v-else>
      <template
        v-for="(childCompo, indexChild) in elementDataRef.children"
        :key="indexChild"
      >
        <SketchCanvasGenericRecursiveComponent :componentData="childCompo" />
      </template>
      <SketchComponentMenu
        :elementData="elementDataRef"
        @update-component-ref="updateComponentRef"
        @copy-to-clipboard="copyToClipBoard"
      />
    </template>
  </Component>
</template>
