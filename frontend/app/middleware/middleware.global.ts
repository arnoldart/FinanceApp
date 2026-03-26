export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthStore()
  const sessionChecked = useState<boolean>('session-checked', () => false)
  const authMode = to.meta.auth as 'required' | 'guest' | undefined

  if (!authMode) {
    return
  }

  if (!sessionChecked.value) {
    await auth.fetchSession()
    sessionChecked.value = true
  }

  if (authMode === 'required' && !auth.isLoggedIn) {
    return navigateTo('/login')
  }

  if (authMode === 'guest' && auth.isLoggedIn) {
    return navigateTo('/dashboard')
  }
})
