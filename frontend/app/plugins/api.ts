import type { NitroFetchOptions } from 'nitropack'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  const baseURL = String(config.public.apiBase || '')
  let isRefreshing = false
  const serverHeaders = import.meta.server ? useRequestHeaders(['cookie']) : undefined

  const api = $fetch.create({
    baseURL,
    credentials: 'include',
    onRequest({ options }) {
      options.credentials = 'include'
      if (import.meta.server && serverHeaders?.cookie) {
        const headers = new Headers(options.headers as HeadersInit | undefined)
        headers.set('cookie', serverHeaders.cookie)
        options.headers = headers
      }
    },
    async onResponseError({ request, options, response }) {
      if (response.status === 401) {
        if (isRefreshing) return

        isRefreshing = true

        try {
          await $fetch('/api/auth/refresh', {
            baseURL,
            method: 'POST',
            credentials: 'include',
            headers: serverHeaders
          })

          isRefreshing = false

          return await $fetch(request, {
            ...(options as NitroFetchOptions<any>),
            baseURL,
            credentials: 'include',
            headers: options.headers ?? serverHeaders
          })
        } catch (err) {
          isRefreshing = false

          const auth = useAuthStore()
          auth.logout()

          navigateTo('/login')
        }
      }
    }
  })

  return {
    provide: {
      api
    }
  }
})
