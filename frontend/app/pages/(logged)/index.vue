<script setup lang="ts">
definePageMeta({
  auth: 'required',
})

await navigateTo('/dashboard', { replace: true })
</script>
