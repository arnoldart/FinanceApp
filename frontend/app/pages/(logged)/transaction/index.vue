<script setup lang="ts">
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import Button from '~/components/ui/button/Button.vue'
import MoneyConverter from '~/lib/MoneyConverter'
import type { Transaction } from '~/types/transaction'

definePageMeta({
  layout: "logged"
})

const { $api } = useNuxtApp()
const router = useRouter()
const transactions = ref<Transaction[]>([])
const activeFilter = ref<'all' | 'income' | 'expense'>('all')

const res = await $api('/api/transaction', {
  method: 'GET',
}) as Transaction[]

transactions.value = res

async function fetchTransactions() {
  const data = await $api('/api/transaction', { method: 'GET' }) as Transaction[]
  transactions.value = data
}

async function deleteTransaction(id: string) {
  if (!confirm('Apakah kamu yakin ingin menghapus transaksi ini?')) return

  try {
    await $api(`/api/transaction/${id}`, { method: 'DELETE' })
    await fetchTransactions()
  } catch (e) {
    console.error('Failed to delete transaction', e)
  }
}

const filteredTransactions = computed(() => {
  if (activeFilter.value === 'income') return transactions.value.filter(t => t.type === 0)
  if (activeFilter.value === 'expense') return transactions.value.filter(t => t.type === 1)
  return transactions.value
})

const totalIncome = computed(() =>
  transactions.value.filter(t => t.type === 0).reduce((sum, t) => sum + t.amount, 0)
)

const totalExpense = computed(() =>
  transactions.value.filter(t => t.type === 1).reduce((sum, t) => sum + t.amount, 0)
)

const totalTransactions = computed(() => transactions.value.length)

function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleDateString('id-ID', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

function formatTime(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleTimeString('id-ID', {
    hour: '2-digit',
    minute: '2-digit',
  })
}
</script>

<template>
  <!-- Page Header -->
  <div class="mb-8 flex flex-col gap-6 sm:flex-row sm:items-end sm:justify-between">
    <div>
      <h1 class="text-2xl font-bold tracking-tight text-foreground">Transaksi</h1>
      <p class="mt-1 text-sm text-muted-foreground">Kelola dan pantau semua aktivitas keuangan kamu</p>
    </div>

    <div class="flex gap-2">
      <Button type="button" variant="outline" class="gap-2 rounded-xl border-border/60 text-sm">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor"
          stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
          <polyline points="7 10 12 15 17 10" />
          <line x1="12" y1="15" x2="12" y2="3" />
        </svg>
        Export
      </Button>
      <NuxtLink to="/transaction/form">
        <Button type="button" class="gap-2 rounded-xl text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor"
            stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <line x1="12" y1="5" x2="12" y2="19" />
            <line x1="5" y1="12" x2="19" y2="12" />
          </svg>
          Tambah Transaksi
        </Button>
      </NuxtLink>
    </div>
  </div>

  <!-- Summary Cards -->
  <section class="mb-8 grid gap-4 sm:grid-cols-3">
    <Card
      class="relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm">
      <div class="absolute inset-0 bg-gradient-to-br from-sky-500/10 via-transparent to-transparent" />
      <CardHeader class="relative pb-2">
        <CardDescription class="text-xs font-semibold uppercase tracking-[0.2em] text-muted-foreground/80">
          Total Transaksi
        </CardDescription>
      </CardHeader>
      <CardContent class="relative">
        <p class="text-3xl font-bold tracking-tight text-foreground">{{ totalTransactions }}</p>
        <p class="mt-1 text-xs text-muted-foreground">Semua transaksi tercatat</p>
      </CardContent>
    </Card>

    <Card
      class="relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm">
      <div class="absolute inset-0 bg-gradient-to-br from-emerald-500/10 via-transparent to-transparent" />
      <CardHeader class="relative pb-2">
        <CardDescription class="text-xs font-semibold uppercase tracking-[0.2em] text-muted-foreground/80">
          Total Pemasukan
        </CardDescription>
      </CardHeader>
      <CardContent class="relative">
        <p class="text-3xl font-bold tracking-tight text-emerald-600">Rp {{ MoneyConverter(totalIncome) }}</p>
        <p class="mt-1 text-xs text-muted-foreground">Akumulasi pemasukan</p>
      </CardContent>
    </Card>

    <Card
      class="relative overflow-hidden rounded-2xl border-border/60 bg-gradient-to-br from-background via-background to-muted/30 shadow-sm">
      <div class="absolute inset-0 bg-gradient-to-br from-rose-500/10 via-transparent to-transparent" />
      <CardHeader class="relative pb-2">
        <CardDescription class="text-xs font-semibold uppercase tracking-[0.2em] text-muted-foreground/80">
          Total Pengeluaran
        </CardDescription>
      </CardHeader>
      <CardContent class="relative">
        <p class="text-3xl font-bold tracking-tight text-rose-600">Rp {{ MoneyConverter(totalExpense) }}</p>
        <p class="mt-1 text-xs text-muted-foreground">Akumulasi pengeluaran</p>
      </CardContent>
    </Card>
  </section>

  <!-- Transactions Table -->
  <Card class="overflow-hidden rounded-2xl border-border/60 shadow-sm">
    <CardHeader class="flex flex-row items-center justify-between gap-4 border-b border-border/40 pb-5">
      <div>
        <CardTitle class="text-lg font-semibold">Riwayat Transaksi</CardTitle>
        <CardDescription class="mt-1">Daftar lengkap semua transaksi yang tercatat</CardDescription>
      </div>

      <!-- Filter Tabs -->
      <div class="flex gap-1 rounded-xl border border-border/50 bg-muted/30 p-1">
        <button
          class="rounded-lg px-4 py-1.5 text-xs font-semibold transition-all duration-200"
          :class="activeFilter === 'all'
            ? 'bg-background text-foreground shadow-sm'
            : 'text-muted-foreground hover:text-foreground'"
          @click="activeFilter = 'all'">
          Semua
        </button>
        <button
          class="rounded-lg px-4 py-1.5 text-xs font-semibold transition-all duration-200"
          :class="activeFilter === 'income'
            ? 'bg-background text-foreground shadow-sm'
            : 'text-muted-foreground hover:text-foreground'"
          @click="activeFilter = 'income'">
          Pemasukan
        </button>
        <button
          class="rounded-lg px-4 py-1.5 text-xs font-semibold transition-all duration-200"
          :class="activeFilter === 'expense'
            ? 'bg-background text-foreground shadow-sm'
            : 'text-muted-foreground hover:text-foreground'"
          @click="activeFilter = 'expense'">
          Pengeluaran
        </button>
      </div>
    </CardHeader>

    <CardContent class="p-0">
      <Table>
        <TableHeader>
          <TableRow class="border-border/40 hover:bg-transparent">
            <TableHead class="pl-6 text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Keterangan
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Rekening
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Tipe
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Tanggal
            </TableHead>
            <TableHead class="text-right text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Jumlah
            </TableHead>
            <TableHead class="pr-6 w-[100px] text-right text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Aksi
            </TableHead>
          </TableRow>
        </TableHeader>

        <TableBody>
          <TableRow v-for="transaction in filteredTransactions" :key="transaction.id"
            class="group relative border-border/30 hover:bg-muted/30 transition-colors cursor-pointer"
            @click="router.push(`/transaction/${transaction.id}`)">
            <TableCell class="pl-6">
              <div class="flex items-center gap-3">
                <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl"
                  :class="transaction.type === 1 ? 'bg-rose-500/10' : 'bg-emerald-500/10'">
                  <svg v-if="transaction.type === 1" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-rose-600"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                    stroke-linejoin="round">
                    <line x1="12" y1="5" x2="12" y2="19" />
                    <polyline points="19 12 12 19 5 12" />
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-emerald-600"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                    stroke-linejoin="round">
                    <line x1="12" y1="19" x2="12" y2="5" />
                    <polyline points="5 12 12 5 19 12" />
                  </svg>
                </div>
                <div>
                  <p class="text-sm font-medium text-foreground">
                    {{ transaction.note || 'Tanpa keterangan' }}
                  </p>
                </div>
              </div>
            </TableCell>

            <TableCell>
              <span
                class="inline-flex items-center rounded-lg border border-border/50 bg-muted/40 px-2.5 py-1 text-xs font-medium text-foreground">
                {{ transaction.walletName }}
              </span>
            </TableCell>

            <TableCell>
              <span class="inline-flex rounded-full px-2.5 py-1 text-xs font-medium" :class="transaction.type === 1
                ? 'bg-rose-500/12 text-rose-700'
                : 'bg-emerald-500/12 text-emerald-700'">
                {{ transaction.type === 1 ? 'Pengeluaran' : 'Pemasukan' }}
              </span>
            </TableCell>

            <TableCell>
              <div>
                <p class="text-sm text-foreground">{{ formatDate(transaction.createdAt) }}</p>
                <p class="text-xs text-muted-foreground">{{ formatTime(transaction.createdAt) }}</p>
              </div>
            </TableCell>

            <TableCell class="text-right">
              <p class="text-sm font-semibold" :class="transaction.type === 1 ? 'text-rose-600' : 'text-emerald-600'">
                {{ transaction.type === 1 ? '-' : '+' }} Rp {{ MoneyConverter(transaction.amount) }}
              </p>
            </TableCell>

            <TableCell class="pr-6 text-right">
              <div class="flex items-center justify-end gap-2">
                <Button type="button" variant="ghost" size="icon" class="h-8 w-8 text-muted-foreground hover:text-foreground hover:bg-muted" @click.stop="router.push(`/transaction/${transaction.id}`)">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                </Button>
                <Button type="button" variant="ghost" size="icon" class="h-8 w-8 text-rose-500 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-500/10" @click.stop="deleteTransaction(transaction.id)">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
                </Button>
              </div>
            </TableCell>
          </TableRow>

          <!-- Empty State -->
          <TableRow v-if="filteredTransactions.length === 0">
            <TableCell colspan="6" class="h-32 text-center">
              <div class="flex flex-col items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-muted-foreground/40" viewBox="0 0 24 24"
                  fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="2" y="5" width="20" height="14" rx="2" />
                  <line x1="2" y1="10" x2="22" y2="10" />
                </svg>
                <p class="text-sm text-muted-foreground">Belum ada transaksi</p>
              </div>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </CardContent>
  </Card>
</template>
