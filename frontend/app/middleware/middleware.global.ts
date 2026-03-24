export default defineNuxtRouteMiddleware(async (to) => {
    const auth = useAuthStore()
    const sessionChecked = useState<boolean>('session-checked', () => false)

    if (!sessionChecked.value) {
        await auth.fetchSession()
        sessionChecked.value = true
    }

    const isProtectedRoute = to.path.startsWith('/logged')
    const isGuestRoute = to.path.startsWith('/auth')

    if (isProtectedRoute && !auth.isLoggedIn) {
        return navigateTo('/login')
    }

    if (isGuestRoute && auth.isLoggedIn) {
        return navigateTo('/dashboard')
    }
})
