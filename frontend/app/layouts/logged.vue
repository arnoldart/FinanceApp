<script setup lang="ts">
import { onClickOutside } from '@vueuse/core'
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarInset,
  SidebarMenu,
  SidebarMenuBadge,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarRail,
  SidebarSeparator,
  SidebarTrigger,
} from '@/components/ui/sidebar'
import {
  ArrowRightLeft,
  ChevronUp,
  CreditCard,
  LayoutDashboard,
  LogOut,
  Settings,
  Sparkles,
  UserCircle2,
  WalletCards,
} from 'lucide-vue-next'

const { $api } = useNuxtApp()
const route = useRoute()
const profileMenuOpen = ref(false)
const profileMenuRef = ref<HTMLElement | null>(null)

const sidebarMenu = [
  {
    name: 'Dashboard',
    description: 'Ringkasan performa keuangan',
    link: '/dashboard',
    icon: LayoutDashboard,
  },
  {
    name: 'Wallet',
    description: 'Kelola akun dan pantau saldo',
    link: '/wallet',
    icon: WalletCards,
  },
  {
    name: 'Transaction',
    description: 'Lihat arus pemasukan dan pengeluaran',
    link: '/transaction',
    icon: ArrowRightLeft,
  },
]

const activeMenu = computed(() => {
  return sidebarMenu.find(item => route.path.startsWith(item.link)) ?? sidebarMenu[0]
})

// const profile = {
//   name: 'Finance User',
//   email: 'you@example.com',
//   role: 'Owner',
// }

function toggleProfileMenu() {
  profileMenuOpen.value = !profileMenuOpen.value
}

function openSettings() {
  profileMenuOpen.value = false
}

async function handleLogout() {
  profileMenuOpen.value = false
  await $api('/api/auth/logout', {method: "POST"})
  await navigateTo("/")
}

onClickOutside(profileMenuRef, () => {
  profileMenuOpen.value = false
})
const profile = await $api('/api/auth/me', { method: "GET" })
</script>

<template>
  <SidebarProvider>
    <Sidebar variant="floating" collapsible="icon" class="border-0">
      <SidebarHeader class="gap-4 p-3">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg"
              class="h-auto rounded-2xl border border-white/60 bg-linear-to-br from-slate-950 via-slate-900 to-slate-800 px-3 py-3 text-slate-50 shadow-lg shadow-slate-950/15 hover:bg-linear-to-br hover:from-slate-950 hover:via-slate-900 hover:to-slate-800 hover:text-white">
              <div
                class="flex aspect-square size-11 items-center justify-center rounded-2xl bg-white/12 text-white ring-1 ring-white/10">
                <CreditCard class="size-5" />
              </div>
              <div class="grid flex-1 text-left leading-tight">
                <span class="truncate text-sm font-semibold tracking-[0.02em]">Finance App</span>
                <span class="truncate text-xs text-slate-300">Personal Money Tracker</span>
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>

        <div
          class="rounded-2xl border border-emerald-200/70 bg-linear-to-br from-emerald-500/12 via-white to-cyan-500/10 p-4 text-sidebar-foreground group-data-[collapsible=icon]:hidden">
          <div class="flex items-start justify-between gap-3">
            <div>
              <p class="text-xs font-semibold uppercase tracking-[0.24em] text-emerald-700/80">
                Focus
              </p>
              <p class="mt-2 text-sm font-semibold text-slate-900">
                {{ activeMenu.name }}
              </p>
              <p class="mt-1 text-xs leading-5 text-slate-600">
                {{ activeMenu.description }}
              </p>
            </div>
            <div class="flex size-10 items-center justify-center rounded-2xl bg-white/80 text-emerald-600 shadow-sm">
              <Sparkles class="size-4" />
            </div>
          </div>
        </div>
      </SidebarHeader>

      <SidebarSeparator class="mx-3 w-auto bg-sidebar-border/70" />

      <SidebarContent class="px-3 py-4">
        <SidebarGroup class="py-0">
          <SidebarGroupLabel class="px-2 text-[11px] font-semibold uppercase tracking-[0.28em] text-slate-500">
            Workspace
          </SidebarGroupLabel>
          <SidebarGroupContent class="mt-3">
            <SidebarMenu class="gap-2">
              <SidebarMenuItem v-for="item in sidebarMenu" :key="item.link">
                <SidebarMenuButton as-child size="lg" :tooltip="item.name"
                  :data-active="route.path.startsWith(item.link)"
                  class="h-auto rounded-2xl border border-transparent px-2 py-2 transition-all duration-200 hover:border-white/80 hover:bg-white/90 hover:shadow-sm data-[active=true]:border-emerald-200/80 data-[active=true]:bg-linear-to-r data-[active=true]:from-emerald-500/15 data-[active=true]:to-cyan-500/10 data-[active=true]:text-slate-950 data-[active=true]:shadow-sm">
                  <NuxtLink :to="item.link" class="flex items-center gap-3">
                    <div
                      class="flex size-10 items-center justify-center rounded-2xl bg-slate-100 text-slate-600 transition-colors duration-200">
                      <component :is="item.icon" class="size-[18px]" />
                    </div>
                    <div class="min-w-0 flex-1 group-data-[collapsible=icon]:hidden">
                      <p class="truncate text-sm font-semibold">
                        {{ item.name }}
                      </p>
                      <p class="truncate text-xs text-slate-500">
                        {{ item.description }}
                      </p>
                    </div>
                  </NuxtLink>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter class="p-3 pt-0">
        <div
          class="rounded-2xl border border-sidebar-border/70 bg-white/75 p-4 shadow-sm backdrop-blur-sm group-data-[collapsible=icon]:hidden">
          <p class="text-xs font-semibold uppercase tracking-[0.24em] text-slate-500">
            Quick Note
          </p>
          <p class="mt-2 text-sm font-semibold text-slate-900">
            Keep tracking every rupiah.
          </p>
          <p class="mt-1 text-xs leading-5 text-slate-600">
            Navigasi dibuat lebih jelas dengan emphasis pada halaman aktif dan ringkasan konteks.
          </p>
        </div>

        <div ref="profileMenuRef" class="relative">
          <button type="button"
            class="flex w-full items-center gap-3 rounded-2xl border border-sidebar-border/70 bg-white/80 p-3 text-left shadow-sm transition-all duration-200 hover:bg-white"
            @click="toggleProfileMenu">
            <div class="flex size-11 items-center justify-center rounded-2xl bg-slate-100 text-slate-600">
              <UserCircle2 class="size-5" />
            </div>
            <div class="min-w-0 flex-1 group-data-[collapsible=icon]:hidden">
              <p class="truncate text-sm font-semibold text-slate-900">
                {{ profile.name }}
              </p>
              <p class="truncate text-xs text-slate-500">
                {{ profile.email }}
              </p>
            </div>
            <ChevronUp
              class="size-4 text-slate-500 transition-transform duration-200 group-data-[collapsible=icon]:hidden"
              :class="profileMenuOpen ? 'rotate-0' : 'rotate-180'" />
          </button>

          <div v-if="profileMenuOpen"
            class="absolute inset-x-0 bottom-[calc(100%+0.5rem)] z-30 overflow-hidden rounded-2xl border border-sidebar-border/70 bg-white/95 p-2 shadow-xl backdrop-blur-xl group-data-[collapsible=icon]:hidden">
            <div class="rounded-xl bg-slate-50/80 px-3 py-3">
              <p class="truncate text-sm font-semibold text-slate-900">
                {{ profile.name }}
              </p>
              <p class="truncate text-xs text-slate-500">
                {{ profile.role }}
              </p>
            </div>

            <div class="mt-2 space-y-1">
              <button type="button"
                class="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-100"
                @click="openSettings">
                <Settings class="size-4" />
                <span>Settings</span>
              </button>

              <button type="button"
                class="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-rose-600 transition-colors hover:bg-rose-50"
                @click="handleLogout">
                <LogOut class="size-4" />
                <span>Logout</span>
              </button>
            </div>
          </div>
        </div>
      </SidebarFooter>

      <SidebarRail />
    </Sidebar>

    <SidebarInset class="bg-muted/30">
      <header
        class="sticky top-0 z-20 flex h-16 shrink-0 items-center justify-between gap-4 border-b border-border/60 bg-background/80 px-4 backdrop-blur-xl transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-12">
        <div class="flex items-center gap-2">
          <SidebarTrigger class="-ml-1" />
          <div>
            <p class="text-sm font-semibold text-foreground">
              {{ activeMenu.name }}
            </p>
            <p class="text-xs text-muted-foreground">
              {{ activeMenu.description }}
            </p>
          </div>
        </div>
      </header>

      <div class="flex flex-1 flex-col gap-4 p-4 md:p-6">
        <NuxtPage />
      </div>
    </SidebarInset>
  </SidebarProvider>
</template>
