<script setup lang="ts">
import { ref, onMounted } from 'vue';

// emits
const emits = defineEmits<{
  (e: 'buttonAction'): void;
}>();

// props
const props = defineProps<{
  message: string;
  description?: string;
  delayInSecond: number;
  labelButtonAction: string;
}>();

// refs
const timer = ref(props.delayInSecond);

// functions
function startCountdown() {
  if (timer.value > 0) {
    setTimeout(() => {
      timer.value = timer.value - 1;
      startCountdown();
    }, 1000);
  }
}

// lifeCycle
onMounted(() => {
  startCountdown();
});
</script>

<template>
  <q-card class="q-pa-lg">
    <q-card-section class="flex flex-center row items-center">
      <div class="text-h6 text-center">
        {{ props.message }}
      </div>
      <div v-if="props.description" class="q-pt-md text-center">
        {{ props.description }}
      </div>
    </q-card-section>

    <q-card-actions align="around">
      <q-btn flat label="Annuler" color="primary" v-close-popup />
      <div class="flew row no-wrap items-center">
        <q-btn
          :disable="timer > 0"
          flat
          :label="props.labelButtonAction"
          :color="timer > 0 ? 'red-3' : 'red'"
          v-close-popup
          @click="emits('buttonAction')"
        />
        <div v-if="timer > 0" class="flex row no-wrap items-center">
          <q-spinner-hourglass color="red-3" size="1.8em" />
          <span class="text-red-3">{{ timer }}</span>
        </div>
      </div>
    </q-card-actions>
  </q-card>
</template>
