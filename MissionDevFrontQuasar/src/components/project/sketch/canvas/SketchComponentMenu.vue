<script setup lang="ts">
import { useQuasar } from 'quasar';
import { ISidebarComponentData } from 'src/utils/sketch-sidebar';
import { getPropsTypeMapByComponent } from 'src/utils/sketch-canvas';
import { ref, watch } from 'vue';

// props
const propsComponent = defineProps<{
  elementData: ISidebarComponentData;
}>();

// emits
const emitsComponents = defineEmits<{
  (e: 'updateComponentRef', elementData: ISidebarComponentData): void;
  (e: 'copyToClipboard', propsAsString: string, content: string): void;
}>();

// consts
const $q = useQuasar();
const propsMapping = getPropsTypeMapByComponent(
  propsComponent.elementData.type
);
propsMapping['class'] = 'string';

// refs
const elementDataRef = ref(propsComponent.elementData);

// function
function copyToClipBoard() {
  const propsAsString = 'TODO';
  const content = 'TODO';
  emitsComponents('copyToClipboard', propsAsString, content);
  $q.notify({
    message: 'Copied to clipboard',
  });
}

// watchs
watch(elementDataRef, (newValue, _oldValue) => {
  emitsComponents('updateComponentRef', newValue);
});
</script>

<template>
  <q-menu touch-position context-menu>
    <q-card class="q-pa-md">
      {{ elementDataRef.type.name }}
      <q-list>
        <template v-for="propsKey in Object.keys(propsMapping)" :key="propsKey">
          <q-item v-if="propsMapping[propsKey] !== undefined">
            <q-input
              v-if="propsMapping[propsKey] === 'string'"
              :label="propsKey"
              v-model="elementDataRef.props[propsKey]"
            />

            <q-toggle
              v-if="propsMapping[propsKey] === 'boolean'"
              :label="propsKey"
              v-model="elementDataRef.props[propsKey]"
            />

            <q-input
              v-if="propsMapping[propsKey] === 'number'"
              :label="propsKey"
              v-model="elementDataRef.props[propsKey]"
              type="number"
            />
          </q-item>
        </template>
      </q-list>

      <q-btn label="Copier dans presse-papier" @click="copyToClipBoard" />
    </q-card>
  </q-menu>
</template>
