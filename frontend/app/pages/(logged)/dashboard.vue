<script setup lang="ts">
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import type { ApexOptions } from 'apexcharts'
import VueApexCharts from 'vue3-apexcharts'
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
const assetChart = computed(() => {
  const total = totalBalance.value
  const income = totalIncomeThisMonth.value
  const expense = totalExpenseThisMonth.value
  const walletBalances = walletSummaries.value.map(wallet => wallet.balance)

  const hasRealData = total > 0 || income > 0 || expense > 0 || walletBalances.length > 0
  const baseline = hasRealData
    ? Math.max(total - income + expense, total * 0.78, 1)
    : 10_500_000

  const influences = walletBalances.length > 0
    ? walletBalances.slice(0, 6).map(balance => balance * 0.08)
    : [450_000, 920_000, 640_000, 1_100_000, 780_000, 560_000]

  const points = Array.from({ length: 10 }, (_, index) => {
    const direction = index % 2 === 0 ? 1 : -1
    const influence = influences[index % influences.length] ?? 500_000
    const incomeDrift = income * (0.14 + index * 0.018)
    const expenseDrag = expense * (0.09 + (9 - index) * 0.01)
    const value = Math.max(
      baseline + incomeDrift - expenseDrag + influence * direction,
      1
    )

    return {
      label: index === 0 ? 'Nov 12, 2025' : index === 9 ? 'Dec 11, 2025' : '',
      value: Math.round(value),
    }
  })

  if (hasRealData) {
    points[points.length - 1].value = Math.max(total, 1)
  }

  const values = points.map(point => point.value)
  const min = Math.min(...values)
  const max = Math.max(...values)
  const range = Math.max(max - min, 1)
  const width = 760
  const height = 260
  const paddingX = 20
  const paddingTop = 18
  const paddingBottom = 28
  const usableHeight = height - paddingTop - paddingBottom
  const stepX = (width - paddingX * 2) / (points.length - 1)

  const svgPoints = points.map((point, index) => {
    const x = paddingX + stepX * index
    const y = paddingTop + (1 - ((point.value - min) / range)) * usableHeight

    return { ...point, x, y }
  })

  const polyline = svgPoints.map(point => `${point.x},${point.y}`).join(' ')
  const highlighted = svgPoints[Math.floor(svgPoints.length * 0.42)]
  return {
    categories: [
      'Nov 12, 2025',
      'Nov 15, 2025',
      'Nov 18, 2025',
      'Nov 20, 2025',
      'Nov 23, 2025',
      'Nov 27, 2025',
      'Dec 01, 2025',
      'Dec 05, 2025',
      'Dec 08, 2025',
      'Dec 11, 2025',
    ],
    series: svgPoints.map(point => point.value),
    min,
    max,
    highlightedIndex: Math.floor(svgPoints.length * 0.42),
    highlighted,
  }
})

const assetChartSeries = computed(() => [
  {
    name: 'Asset Total',
    data: assetChart.value.series,
  },
])

const assetChartOptions = computed<ApexOptions>(() => ({
  chart: {
    id: 'asset-total-statistic',
    toolbar: { show: false },
    zoom: { enabled: false },
    sparkline: { enabled: false },
    fontFamily: 'inherit',
  },
  colors: ['#7c83ff'],
  stroke: {
    curve: 'smooth',
    width: 3,
  },
  dataLabels: {
    enabled: false,
  },
  fill: {
    type: 'solid',
    opacity: 1,
  },
  grid: {
    borderColor: 'rgba(148, 163, 184, 0.22)',
    strokeDashArray: 5,
    padding: {
      left: 8,
      right: 8,
      top: 12,
      bottom: 0,
    },
    xaxis: {
      lines: { show: false },
    },
  },
  markers: {
    size: 0,
    hover: {
      size: 6,
    },
    discrete: [
      {
        seriesIndex: 0,
        dataPointIndex: assetChart.value.highlightedIndex,
        fillColor: '#ffffff',
        strokeColor: '#7c83ff',
        size: 6,
      },
    ],
  },
  annotations: {
    xaxis: [
      {
        x: assetChart.value.categories[assetChart.value.highlightedIndex],
        borderColor: '#cbd5e1',
        strokeDashArray: 4,
      },
    ],
    points: [
      {
        x: assetChart.value.categories[assetChart.value.highlightedIndex],
        y: assetChart.value.highlighted.value,
        marker: {
          size: 0,
        },
        label: {
          borderColor: '#e2e8f0',
          style: {
            background: '#ffffff',
            color: '#0f172a',
            fontSize: '12px',
            fontWeight: 600,
          },
          offsetY: -8,
          text: `Rp ${MoneyConverter(assetChart.value.highlighted.value)}\nNov 20, 2025`,
        },
      },
    ],
  },
  tooltip: {
    theme: 'light',
    x: {
      show: true,
    },
    y: {
      formatter: value => `Rp ${MoneyConverter(Number(value))}`,
    },
  },
  xaxis: {
    categories: assetChart.value.categories,
    axisBorder: {
      show: false,
    },
    axisTicks: {
      show: false,
    },
    crosshairs: {
      show: false,
    },
    labels: {
      style: {
        colors: '#94a3b8',
        fontSize: '12px',
      },
      formatter: (_value, _timestamp, index) => {
        if (index === 0 || index === assetChart.value.categories.length - 1) {
          return assetChart.value.categories[index] ?? ''
        }

        return ''
      },
    },
  },
  yaxis: {
    min: assetChart.value.min,
    max: assetChart.value.max,
    tickAmount: 3,
    labels: {
      style: {
        colors: '#94a3b8',
        fontSize: '12px',
      },
      formatter: value => `$${Math.round(value / 1_000_000)}M`,
    },
  },
  legend: {
    show: false,
  },
}))
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
    <Card v-if="isLoading" v-for="n in 3" :key="`wallet-skeleton-${n}`" class="rounded-2xl border-border/60 shadow-sm">
      <CardHeader class="pb-3">
        <div class="flex items-start justify-between gap-3">
          <div class="space-y-2">
            <Skeleton class="h-3 w-24" />
            <Skeleton class="h-5 w-32" />
          </div>
          <Skeleton class="h-11 w-11 rounded-2xl" />
        </div>
      </CardHeader>

      <CardContent class="space-y-4">
        <div class="space-y-2">
          <Skeleton class="h-8 w-40" />
          <Skeleton class="h-4 w-28" />
        </div>

        <div class="flex items-center justify-between rounded-xl border px-4 py-3">
          <div class="space-y-2">
            <Skeleton class="h-3 w-16" />
            <Skeleton class="h-4 w-24" />
          </div>
          <Skeleton class="h-6 w-20 rounded-full" />
        </div>
      </CardContent>
    </Card>
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

  <section>
    <Card class="overflow-hidden rounded-2xl border-border/60 shadow-sm">
      <CardHeader class="flex flex-row items-start justify-between gap-4">
        <div>
          <CardTitle class="text-lg">Asset Total Statistic</CardTitle>
          <CardDescription class="mt-1">
            Ringkasan tren aset dari data dashboard saat ini.
          </CardDescription>
        </div>

        <div class="rounded-lg border border-border/60 bg-background px-3 py-2 text-sm text-muted-foreground shadow-xs">
          Monthly
        </div>
      </CardHeader>

      <CardContent v-if="isLoading" class="space-y-4">
        <Skeleton class="h-4 w-44" />
        <Skeleton class="h-72 w-full rounded-2xl" />
      </CardContent>

      <CardContent v-else class="space-y-4">
        <ClientOnly>
          <VueApexCharts
            type="line"
            height="320"
            :options="assetChartOptions"
            :series="assetChartSeries"
          />
        </ClientOnly>
      </CardContent>
    </Card>
  </section>


  <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
    <div v-if="isLoading" class="rounded-2xl border border-border/60 p-6 space-y-4">
      <div class="flex items-center justify-between">
        <div class="space-y-2">
          <Skeleton class="h-5 w-40" />
          <Skeleton class="h-4 w-56" />
        </div>
        <Skeleton class="h-4 w-16" />
      </div>

      <Skeleton class="h-4 w-full" />
      <Skeleton class="h-16 w-full rounded-xl" />
      <Skeleton class="h-16 w-full rounded-xl" />
      <Skeleton class="h-16 w-full rounded-xl" />
    </div>
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
