<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})
</script>

<template>
  <Card class="w-full max-w-sm">
    <CardHeader>
      <CardTitle class="text-center">Reset your password account</CardTitle>
    </CardHeader>
    <CardContent>
      <form id="register-form" class="space-y-4" @submit.prevent>
        <div class="grid w-full items-center gap-4">
          <div class="flex flex-col space-y-1.5">
            <div class="flex items-center">
              <Label for="password">Password</Label>
            </div>
            <Input id="password" type="password" />
          </div>
          <div class="flex flex-col space-y-1.5">
            <div class="flex items-center">
              <Label for="confirmPassword">Confirm Password</Label>
            </div>
            <Input id="confirmPassword" type="password" />
          </div>
        </div>
      </form>
    </CardContent>
    <CardFooter class="flex flex-col gap-2">
      <Button type="submit" form="register-form" class="w-full">
        Reset Password
      </Button>
    </CardFooter>
  </Card>
</template>
