<script setup lang="ts">
import WalletForm from '~/components/wallet/WalletForm.vue'

definePageMeta({
    layout: "logged",
    auth: 'required',
})
</script>

<template>
    <WalletForm />
</template>
