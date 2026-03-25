<script setup lang="ts">
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import MoneyConverter from '~/lib/MoneyConverter'

definePageMeta({
  layout: "logged"
})

const summaryCards = [
  {
    title: 'Total Keuangan',
    amount: 'Rp 1.000.000',
    description: 'Akumulasi dari seluruh rekening aktif',
    accent: 'from-emerald-500/20 via-teal-500/10 to-transparent',
    glow: 'bg-emerald-500',
    tag: 'Semua akun',
  },
  {
    title: 'Total BCA',
    amount: 'Rp 1.000.000',
    description: 'Saldo utama untuk transaksi harian',
    accent: 'from-sky-500/20 via-cyan-500/10 to-transparent',
    glow: 'bg-sky-500',
    tag: 'Bank BCA',
  },
  {
    title: 'Total BRI',
    amount: 'Rp 1.000.000',
    description: 'Dipakai untuk pemasukan dan tabungan',
    accent: 'from-indigo-500/20 via-blue-500/10 to-transparent',
    glow: 'bg-indigo-500',
    tag: 'Bank BRI',
  },
  {
    title: 'Total BTN',
    amount: 'Rp 1.000.000',
    description: 'Disiapkan untuk target keuangan khusus',
    accent: 'from-amber-500/20 via-orange-500/10 to-transparent',
    glow: 'bg-amber-500',
    tag: 'Bank BTN',
  },
]

const listTransaction = []

for (let i = 0; i < 10; i++) {
  listTransaction.push({ name: "Bayar Makan", transaction_type: "pengeluaran", amount: "10000" })
}

</script>

<template>
  <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
    <Card v-for="card in summaryCards" :key="card.title"
      class="group relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl">
      <div class="absolute inset-0 bg-gradient-to-br opacity-100" :class="card.accent" />
      <div class="absolute -right-10 -top-10 h-28 w-28 rounded-full bg-white/35 blur-3xl" />

      <CardHeader class="relative gap-4 pb-3">
        <div class="flex items-start justify-between gap-3">
          <div>
            <CardDescription class="mb-2 text-xs font-semibold uppercase tracking-[0.24em] text-muted-foreground/80">
              {{ card.tag }}
            </CardDescription>
            <CardTitle class="text-lg font-semibold text-foreground">
              {{ card.title }}
            </CardTitle>
          </div>

          <div
            class="flex h-11 w-11 items-center justify-center rounded-2xl border border-white/40 bg-white/70 backdrop-blur-sm">
            <span class="h-3 w-3 rounded-full" :class="card.glow" />
          </div>
        </div>
      </CardHeader>

      <CardContent class="relative space-y-4">
        <div>
          <p class="text-3xl font-semibold tracking-tight text-foreground">
            {{ card.amount }}
          </p>
          <p class="mt-2 max-w-[22ch] text-sm leading-6 text-muted-foreground">
            {{ card.description }}
          </p>
        </div>

        <div
          class="flex items-center justify-between rounded-xl border border-white/50 bg-white/65 px-4 py-3 backdrop-blur-sm">
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              Status
            </p>
            <p class="mt-1 text-sm font-medium text-foreground">
              Stabil bulan ini
            </p>
          </div>

          <div class="rounded-full bg-foreground px-3 py-1 text-xs font-semibold text-background">
            +12.4%
          </div>
        </div>
      </CardContent>
    </Card>
  </section>

  <section>

  </section>

  <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
    <Card>
      <CardHeader class="flex flex-row items-center justify-between">
        <div>
          <CardTitle class="text-lg">Transactions</CardTitle>
          <CardDescription class="mt-1">
            Aktivitas transaksi terbaru
          </CardDescription>
        </div>

        <NuxtLink
          to="/transaction"
          class="text-sm font-semibold text-muted-foreground transition-colors hover:text-foreground"
        >
          See All
        </NuxtLink>
      </CardHeader>

      <CardContent class="space-y-3">
        <div class="hidden grid-cols-[minmax(0,1.5fr)_minmax(0,1fr)_minmax(0,1fr)] border-b border-border/60 pb-3 text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground md:grid">
          <p>Nama Transaksi</p>
          <p>Tipe</p>
          <p class="text-right">Jumlah</p>
        </div>

        <div
          v-for="(transaction, index) in listTransaction"
          :key="`${transaction.name}-${index}`"
          class="grid gap-2 rounded-xl border border-border/50 px-4 py-3 md:grid-cols-[minmax(0,1.5fr)_minmax(0,1fr)_minmax(0,1fr)] md:items-center"
        >
          <p class="truncate text-sm font-medium text-foreground">
            {{ transaction.name }}
          </p>

          <div>
            <span
              class="inline-flex rounded-full px-2.5 py-1 text-xs font-medium capitalize"
              :class="transaction.transaction_type === 'pengeluaran'
                ? 'bg-rose-500/12 text-rose-700'
                : 'bg-emerald-500/12 text-emerald-700'"
            >
              {{ transaction.transaction_type }}
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
