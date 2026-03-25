<script setup lang="ts">
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
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
import type { WalletSummary } from '~/types/dashboard' // using dashboard's WalletSummary or the new wallet type

definePageMeta({
  layout: "logged"
})

const { $api } = useNuxtApp()
const router = useRouter()

// We can fetch from /api/wallet or use the summary from /api/dashboard. 
// Assuming a dedicated /api/wallet exists that returns an array of wallets.
const wallets = ref<WalletSummary[]>([])

async function fetchWallets() {
  try {
    const data = await $api('/api/dashboard', { method: 'GET' }) as any
    wallets.value = data.walletSummaries || []
  } catch (e) {
    console.error('Failed to load wallets', e)
  }
}

// Initial fetch
await fetchWallets()

async function deleteWallet(id: string) {
  if (!confirm('Apakah kamu yakin ingin menghapus rekening ini? Semua transaksi terkait mungkin akan terpengaruh.')) return

  try {
    await $api(`/api/wallet/${id}`, { method: 'DELETE' })
    await fetchWallets()
  } catch (e) {
    console.error('Failed to delete wallet', e)
  }
}

</script>

<template>
  <!-- Header Section -->
  <div class="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
    <div>
      <h1 class="text-2xl font-bold tracking-tight text-foreground">Rekening</h1>
      <p class="mt-1 text-sm text-muted-foreground">Kelola daftar rekening dan e-wallet kamu</p>
    </div>
    <NuxtLink to="/wallet/form">
      <Button class="w-full sm:w-auto rounded-xl shadow-sm">
        <svg xmlns="http://www.w3.org/2000/svg" class="mr-2 h-4 w-4" viewBox="0 0 24 24" fill="none"
          stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M5 12h14" />
          <path d="M12 5v14" />
        </svg>
        Tambah Rekening
      </Button>
    </NuxtLink>
  </div>

  <!-- Wallet Table -->
  <Card class="overflow-hidden rounded-2xl border-border/60 shadow-sm">
    <CardHeader class="border-b border-border/40 bg-muted/10 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <CardTitle class="text-lg font-semibold">Daftar Rekening</CardTitle>
          <CardDescription class="mt-1">Semua rekening yang kamu miliki</CardDescription>
        </div>
      </div>
    </CardHeader>

    <CardContent class="p-0">
      <Table>
        <TableHeader>
          <TableRow class="border-border/40 hover:bg-transparent">
            <TableHead class="pl-6 text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Nama Rekening
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80 text-center">
              Total Transaksi
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Pemasukan (Bulan Ini)
            </TableHead>
            <TableHead class="text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Pengeluaran (Bulan Ini)
            </TableHead>
            <TableHead class="text-right text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Saldo Saat Ini
            </TableHead>
            <TableHead class="pr-6 w-[100px] text-right text-xs font-semibold uppercase tracking-[0.18em] text-muted-foreground/80">
              Aksi
            </TableHead>
          </TableRow>
        </TableHeader>

        <TableBody>
          <TableRow v-for="wallet in wallets" :key="wallet.walletId"
            class="group relative border-border/30 hover:bg-muted/30 transition-colors cursor-pointer"
            @click="router.push(`/wallet/${wallet.walletId}`)">
            
            <!-- Wallet Name -->
            <TableCell class="pl-6">
              <div class="flex items-center gap-3">
                <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-sky-500/10">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-sky-600" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 12V7H5a2 2 0 0 1 0-4h14v4"/><path d="M3 5v14a2 2 0 0 0 2 2h16v-5"/><path d="M18 12a2 2 0 0 0 0 4h4v-4Z"/></svg>
                </div>
                <div>
                  <p class="text-sm font-medium text-foreground">
                    {{ wallet.walletName }}
                  </p>
                </div>
              </div>
            </TableCell>

            <!-- Tx Count -->
            <TableCell class="text-center">
              <span class="inline-flex items-center rounded-lg border border-border/50 bg-muted/40 px-2.5 py-1 text-xs font-medium text-foreground">
                {{ wallet.transactionCount }} trx
              </span>
            </TableCell>

            <!-- Income -->
            <TableCell>
              <p class="text-sm font-medium text-emerald-600">
                + Rp {{ MoneyConverter(wallet.incomeThisMonth) }}
              </p>
            </TableCell>

            <!-- Expense -->
            <TableCell>
              <p class="text-sm font-medium text-rose-600">
                - Rp {{ MoneyConverter(wallet.expenseThisMonth) }}
              </p>
            </TableCell>

            <!-- Balance -->
            <TableCell class="text-right">
              <p class="text-sm font-semibold text-foreground">
                Rp {{ MoneyConverter(wallet.balance) }}
              </p>
            </TableCell>

            <!-- Actions -->
            <TableCell class="pr-6 text-right">
              <div class="flex items-center justify-end gap-2">
                <Button type="button" variant="ghost" size="icon" class="h-8 w-8 text-muted-foreground hover:text-foreground hover:bg-muted" @click.stop="router.push(`/wallet/${wallet.walletId}`)">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                </Button>
                <Button type="button" variant="ghost" size="icon" class="h-8 w-8 text-rose-500 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-500/10" @click.stop="deleteWallet(wallet.walletId)">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
                </Button>
              </div>
            </TableCell>
          </TableRow>

          <!-- Empty State -->
          <TableRow v-if="wallets.length === 0">
            <TableCell colspan="6" class="h-32 text-center">
              <div class="flex flex-col items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-muted-foreground/40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M21 12V7H5a2 2 0 0 1 0-4h14v4"/><path d="M3 5v14a2 2 0 0 0 2 2h16v-5"/><path d="M18 12a2 2 0 0 0 0 4h4v-4Z"/></svg>
                <p class="text-sm font-medium text-muted-foreground">Belum ada rekening</p>
                <p class="text-xs text-muted-foreground/80">Silakan tambah rekening baru terlebih dahulu.</p>
              </div>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </CardContent>
  </Card>
</template>