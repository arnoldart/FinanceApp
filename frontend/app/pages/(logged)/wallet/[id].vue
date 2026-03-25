<script setup lang="ts">
import WalletForm from '~/components/wallet/WalletForm.vue'

definePageMeta({
    layout: "logged"
})

const route = useRoute()
const walletId = computed(() => route.params.id as string)
</script>

<template>
    <WalletForm :walletId="walletId" />
</template>