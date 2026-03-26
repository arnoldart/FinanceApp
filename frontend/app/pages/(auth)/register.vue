<script setup lang="ts">
definePageMeta({
  layout: 'auth',
  auth: 'guest',
})
</script>

<template>
  <Card class="w-full max-w-sm">
    <CardHeader>
      <CardTitle class="text-center">Register your account</CardTitle>
    </CardHeader>
    <CardContent>
      <form id="register-form" class="space-y-4" @submit.prevent>
        <div class="grid w-full items-center gap-4">
          <div class="flex flex-col space-y-1.5">
            <Label for="username">Username</Label>
            <Input id="username" type="text" placeholder="username" />
          </div>
          <div class="flex flex-col space-y-1.5">
            <Label for="email">Email</Label>
            <Input id="email" type="email" placeholder="me@example.com" />
          </div>
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
          <div class="flex flex-col justify-center items-center">
            Already account ?
            <NuxtLink to="/login" class="text-sm underline">Sign In</NuxtLink>
          </div>
        </div>
      </form>
    </CardContent>
    <CardFooter class="flex flex-col gap-2">
      <Button type="submit" form="register-form" class="w-full">
        Register
      </Button>
    </CardFooter>
  </Card>
</template>
