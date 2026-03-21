<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})
</script>

<template>
  <Card class="w-full max-w-sm">
    <CardHeader>
      <CardTitle class="text-center">Login to your account</CardTitle>
    </CardHeader>
    <CardContent>
      <form id="login-form" class="space-y-4" @submit.prevent>
        <div class="grid w-full items-center gap-4">
          <div class="flex flex-col space-y-1.5">
            <Label for="email">Email</Label>
            <Input id="email" type="email" placeholder="m@example.com" />
          </div>
          <div class="flex flex-col space-y-1.5">
            <div class="flex items-center">
              <Label for="password">Password</Label>
              <NuxtLink to="/auth/forgot-password" class="ml-auto inline-block text-sm underline">
                Forgot your password?
              </NuxtLink>
            </div>
            <Input id="password" type="password" />
          </div>
          <div class="flex flex-col justify-center items-center">
            Don't have account ?
            <NuxtLink to="/auth/register" class="text-sm underline">Sign Up</NuxtLink>
          </div>
        </div>
      </form>
    </CardContent>
    <CardFooter class="flex flex-col gap-2">
      <Button type="submit" form="login-form" class="w-full">
        Login
      </Button>
    </CardFooter>
  </Card>
</template>
