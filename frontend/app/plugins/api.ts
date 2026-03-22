export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  const baseURL = String(config.public.apiBase || '')

  const api = $fetch.create({
    baseURL,
    credentials: 'include',
    onRequest({ options }) {
      options.credentials = 'include'
    },
    onResponseError({ response }) {
      if (response.status === 401) {
        const auth = useAuthStore()
        auth.logout()
      }
    }
  })

  return {
    provide: {
      api
    }
  }
})
