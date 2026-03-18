<script setup lang="ts">
import Button from './components/ui/button/Button.vue';

const counter = ref(0);

const handleClick = () => {
  counter.value++;
};

</script>

<template>
  <div>
    <!-- <NuxtRouteAnnouncer />
    <NuxtWelcome /> -->
    <Button class="bg-gray-500" @click="handleClick">Click Me</Button>
    <p>Count: {{ counter }}</p>
  </div>
</template>
