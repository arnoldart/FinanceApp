<script setup lang="ts">
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import MoneyConverter from '~/lib/MoneyConverter'
import type { DashboardResponse } from '~/types/dashboard';

definePageMeta({
  layout: 'logged',
  auth: 'required',
})

const walletAccents: Array<{ accent: string; glow: string }> = [
  { accent: 'from-sky-500/20 via-cyan-500/10 to-transparent', glow: 'bg-sky-500' },
  { accent: 'from-indigo-500/20 via-blue-500/10 to-transparent', glow: 'bg-indigo-500' },
  { accent: 'from-amber-500/20 via-orange-500/10 to-transparent', glow: 'bg-amber-500' },
  { accent: 'from-rose-500/20 via-pink-500/10 to-transparent', glow: 'bg-rose-500' },
]

const { $api } = useNuxtApp()

const { data: dashboardData, pending: isLoading, error } = await useAsyncData(
  'dashboard-data',
  () => $api('/api/dashboard', { method: 'GET' }) as Promise<DashboardResponse>,
  {
    lazy: true,
    dedupe: 'defer',
    deep: false,
  }
)

const totalBalance = computed(() => dashboardData.value?.totalBalance ?? 0)
const totalIncomeThisMonth = computed(() => dashboardData.value?.totalIncomeThisMonth ?? 0)
const totalExpenseThisMonth = computed(() => dashboardData.value?.totalExpenseThisMonth ?? 0)
const recentTransactions = computed(() => dashboardData.value?.recentTransactions ?? [])
const walletSummaries = computed(() => dashboardData.value?.walletSummaries ?? [])
</script>

<template>
  <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
    <!-- Card 1: Total Keuangan -->
    <Card
      class="group relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl">
      <div class="absolute inset-0 bg-gradient-to-br from-emerald-500/20 via-teal-500/10 to-transparent opacity-100" />
      <div class="absolute -right-10 -top-10 h-28 w-28 rounded-full bg-white/35 blur-3xl" />

      <CardHeader class="relative gap-4 pb-3">
        <div class="flex items-start justify-between gap-3">
          <div>
            <CardDescription class="mb-2 text-xs font-semibold uppercase tracking-[0.24em] text-muted-foreground/80">
              Semua akun
            </CardDescription>
            <CardTitle class="text-lg font-semibold text-foreground">
              Total Keuangan
            </CardTitle>
          </div>

          <div
            class="flex h-11 w-11 items-center justify-center rounded-2xl border border-white/40 bg-white/70 backdrop-blur-sm">
            <span class="h-3 w-3 rounded-full bg-emerald-500" />
          </div>
        </div>
      </CardHeader>

      <CardContent class="relative space-y-4">
        <div>
          <p class="text-3xl font-semibold tracking-tight text-foreground">
            Rp {{ MoneyConverter(totalBalance) }}
          </p>
          <p class="mt-2 max-w-[22ch] text-sm leading-6 text-muted-foreground">
            Akumulasi dari seluruh rekening aktif
          </p>
        </div>

        <div
          class="flex items-center justify-between rounded-xl border border-white/50 bg-white/65 px-4 py-3 backdrop-blur-sm">
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              Bulan Ini
            </p>
            <p class="mt-1 text-sm font-medium text-emerald-600">
              + Rp {{ MoneyConverter(totalIncomeThisMonth) }}
            </p>
          </div>

          <div class="rounded-full bg-rose-500/12 px-3 py-1 text-xs font-semibold text-rose-700">
            - Rp {{ MoneyConverter(totalExpenseThisMonth) }}
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Wallet Cards -->
    <Skeleton v-if="isLoading" />
    <Card v-else v-for="(wallet, index) in walletSummaries" :key="wallet.walletId"
      class="group relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl">
      <div class="absolute inset-0 bg-gradient-to-br opacity-100"
        :class="walletAccents[index % walletAccents.length].accent" />
      <div class="absolute -right-10 -top-10 h-28 w-28 rounded-full bg-white/35 blur-3xl" />

      <CardHeader class="relative gap-4 pb-3">
        <div class="flex items-start justify-between gap-3">
          <div>
            <CardDescription class="mb-2 text-xs font-semibold uppercase tracking-[0.24em] text-muted-foreground/80">
              {{ wallet.transactionCount }} transaksi
            </CardDescription>
            <CardTitle class="text-lg font-semibold text-foreground">
              {{ wallet.walletName }}
            </CardTitle>
          </div>

          <div
            class="flex h-11 w-11 items-center justify-center rounded-2xl border border-white/40 bg-white/70 backdrop-blur-sm">
            <span class="h-3 w-3 rounded-full" :class="walletAccents[index % walletAccents.length].glow" />
          </div>
        </div>
      </CardHeader>

      <CardContent class="relative space-y-4">
        <div>
          <p class="text-3xl font-semibold tracking-tight text-foreground">
            Rp {{ MoneyConverter(wallet.balance) }}
          </p>
          <p class="mt-2 max-w-[22ch] text-sm leading-6 text-muted-foreground">
            Saldo rekening {{ wallet.walletName }}
          </p>
        </div>

        <div
          class="flex items-center justify-between rounded-xl border border-white/50 bg-white/65 px-4 py-3 backdrop-blur-sm">
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              Bulan Ini
            </p>
            <p class="mt-1 text-sm font-medium text-emerald-600">
              + Rp {{ MoneyConverter(wallet.incomeThisMonth) }}
            </p>
          </div>

          <div class="rounded-full bg-rose-500/12 px-3 py-1 text-xs font-semibold text-rose-700">
            - Rp {{ MoneyConverter(wallet.expenseThisMonth) }}
          </div>
        </div>
      </CardContent>
    </Card>
  </section>


  <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
    <Skeleton v-if="isLoading" class="h-md w-[342px]" />
    <Card v-else>
      <CardHeader class="flex flex-row items-center justify-between">
        <div>
          <CardTitle class="text-lg">Transactions</CardTitle>
          <CardDescription class="mt-1">
            Aktivitas transaksi terbaru
          </CardDescription>
        </div>

        <NuxtLink to="/transaction"
          class="text-sm font-semibold text-muted-foreground transition-colors hover:text-foreground">
          See All
        </NuxtLink>
      </CardHeader>

      <CardContent class="space-y-3">
        <div
          class="hidden grid-cols-[minmax(0,1.5fr)_minmax(0,1fr)_minmax(0,1fr)] border-b border-border/60 pb-3 text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground md:grid">
          <p>Nama Transaksi</p>
          <p>Tipe</p>
          <p class="text-right">Jumlah</p>
        </div>

        <div v-for="(transaction, index) in recentTransactions" :key="`${transaction.note}-${index}`"
          class="grid gap-2 rounded-xl border border-border/50 px-4 py-3 md:grid-cols-[minmax(0,1.5fr)_minmax(0,1fr)_minmax(0,1fr)] md:items-center">
          <p class="truncate text-sm font-medium text-foreground">
            {{ transaction.note }}
          </p>

          <div>
            <span class="inline-flex rounded-full px-2.5 py-1 text-xs font-medium capitalize" :class="transaction.type === 1
              ? 'bg-rose-500/12 text-rose-700'
              : 'bg-emerald-500/12 text-emerald-700'">
              {{ transaction.type === 1 ? "Pengeluaran" : "Pemasukan" }}
            </span>
          </div>

          <p class="text-sm font-semibold text-foreground md:text-right">
            Rp {{ MoneyConverter(transaction.amount) }}
          </p>
        </div>
      </CardContent>
    </Card>
  </section>
</template>
