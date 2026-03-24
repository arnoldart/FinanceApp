<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})
</script>

<template>
  <Card class="w-full max-w-sm">
    <CardHeader>
      <CardTitle class="text-center">Forgot Password</CardTitle>
    </CardHeader>
    <CardContent>
      <form id="register-form" class="space-y-4" @submit.prevent>
        <div class="grid w-full items-center gap-4">
          <div class="flex flex-col space-y-1.5">
            <Label for="email">Email</Label>
            <Input id="email" type="email" placeholder="me@example.com" />
          </div>
        </div>
      </form>
    </CardContent>
    <CardFooter class="flex flex-col gap-2">
      <Button type="submit" form="register-form" class="w-full">
        Submit
      </Button>
    </CardFooter>
  </Card>
</template>
