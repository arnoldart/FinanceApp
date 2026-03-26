<script setup lang="ts">
import TransactionForm from '~/components/transaction/TransactionForm.vue'

definePageMeta({
    layout: "logged",
    auth: 'required',
})

const route = useRoute()
const transactionId = computed(() => route.params.id as string)
</script>

<template>
    <TransactionForm :transactionId="transactionId" />
</template>
