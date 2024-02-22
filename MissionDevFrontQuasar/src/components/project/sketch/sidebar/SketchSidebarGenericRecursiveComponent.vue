<script setup lang="ts">
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
// components
import SketchSidebarGenericRecursiveComponent from 'src/components/project/sketch/sidebar/SketchSidebarGenericRecursiveComponent.vue';

// props
const propsComponent = defineProps<{
  componentData: ISidebarComponentData;
}>();
</script>

<template>
  <Component
    v-bind="propsComponent.componentData.props"
    :is="propsComponent.componentData.type"
  >
    <template v-if="propsComponent.componentData.rawContent">{{
      propsComponent.componentData.rawContent
    }}</template>
    <template v-else>
      <template
        v-for="(childCompo, indexChild) in propsComponent.componentData
          .children"
        :key="indexChild"
      >
        <SketchSidebarGenericRecursiveComponent :componentData="childCompo" />
      </template>
    </template>
  </Component>
</template>
