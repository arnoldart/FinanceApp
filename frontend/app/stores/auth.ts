import type { AuthLoginRequest, AuthLoginResponse } from "~/types/auth"

export const useAuthStore = defineStore('auth', () => {
  const isLoggedIn = useState<boolean>('is-logged-in', () => false)

  function logout() {
    isLoggedIn.value = false
  }

  async function fetchSession() {
    const { $api } = useNuxtApp()
    try {
      await $api('/api/auth/me', { method: 'GET' })
      isLoggedIn.value = true
    } catch {
      isLoggedIn.value = false
    }
  }

  async function login(payload: AuthLoginRequest) {
    const { $api } = useNuxtApp()
    await $api<AuthLoginResponse>('/api/auth/login', {
      method: 'POST',
      body: payload
    })
    isLoggedIn.value = true
  }

  return { isLoggedIn, login, logout, fetchSession }

})