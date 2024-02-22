<script setup lang="ts">
import { ref, watch } from 'vue';

// props
const propsComponent = defineProps<{
  rawContent: string;
}>();

// emits
const emitsComponents = defineEmits<{
  (e: 'update-component-raw-content', newRawContent: string): void;
}>();

// refs
const rawContentRef = ref(propsComponent.rawContent);

// watchs
watch(rawContentRef, (newValue, _oldValue) => {
  if (newValue.length === 0) {
    emitsComponents('update-component-raw-content', '_');
  } else {
    emitsComponents('update-component-raw-content', newValue);
  }
});
</script>

<template>
  <q-menu touch-position context-menu>
    <q-card class="q-pa-md">
      <q-input
        v-model="rawContentRef"
        label="Contenu textuel"
        type="textarea"
      />
    </q-card>
  </q-menu>
</template>
