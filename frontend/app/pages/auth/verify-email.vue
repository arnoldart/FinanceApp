<script setup lang="ts">
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSeparator,
  InputOTPSlot,
} from '@/components/ui/input-otp'

definePageMeta({
  layout: 'auth'
})

</script>

<template>
  <Card class="w-full max-w-sm">
    <CardHeader>
      <CardTitle class="text-center">Enter your OTP</CardTitle>
    </CardHeader>
    <CardContent class="flex justify-center">
      <InputOTP :maxlength="6">
        <InputOTPGroup>
          <InputOTPSlot :index="0" />
          <InputOTPSlot :index="1" />
          <InputOTPSlot :index="2" />
        </InputOTPGroup>
        <InputOTPSeparator />
        <InputOTPGroup>
          <InputOTPSlot :index="3" />
          <InputOTPSlot :index="4" />
          <InputOTPSlot :index="5" />
        </InputOTPGroup>
      </InputOTP>
    </CardContent>
    <CardFooter class="flex flex-col gap-2">
      <Button type="submit" form="register-form" class="w-full">
        Submit
      </Button>
    </CardFooter>
  </Card>
</template>
