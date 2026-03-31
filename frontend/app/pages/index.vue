<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  ArrowRight,
  ArrowUpRight,
  BadgeDollarSign,
  BanknoteArrowDown,
  BellRing,
  ChartColumnBig,
  ChartNoAxesCombined,
  ChartPie,
  CircleDollarSign,
  CreditCard,
  Fingerprint,
  Layers,
  PiggyBank,
  Receipt,
  Send,
  ShieldCheck,
  Sparkles,
  Target,
  TrendingUp,
  Wallet,
  WalletCards,
  Zap,
} from 'lucide-vue-next'

const isLoaded = ref(false)
onMounted(() => {
  setTimeout(() => (isLoaded.value = true), 100)
})

const features = [
  {
    icon: WalletCards,
    title: 'Multi-Wallet',
    description: 'Pisahkan uang harian, tabungan, dana darurat dengan satu tampilan.',
    accent: 'emerald',
  },
  {
    icon: ChartNoAxesCombined,
    title: 'Analisis Cashflow',
    description: 'Lihat pola pengeluaran dan tren pemasukan secara visual.',
    accent: 'cyan',
  },
  {
    icon: BellRing,
    title: 'Smart Reminder',
    description: 'Pengingat otomatis untuk cicilan, langganan, dan target saving.',
    accent: 'violet',
  },
  {
    icon: Target,
    title: 'Budget Tracker',
    description: 'Atur limit budget per kategori dan pantau progress real-time.',
    accent: 'amber',
  },
  {
    icon: Fingerprint,
    title: 'Privasi Terjaga',
    description: 'Data terenkripsi end-to-end. Hanya kamu yang bisa mengakses.',
    accent: 'rose',
  },
  {
    icon: Layers,
    title: 'Kategori Fleksibel',
    description: '50+ kategori siap pakai atau buat custom sesuai kebutuhanmu.',
    accent: 'sky',
  },
]

const steps = [
  {
    num: '01',
    title: 'Buat Akun Gratis',
    text: 'Daftar dalam hitungan detik dan setup wallet pertamamu.',
    icon: Wallet,
  },
  {
    num: '02',
    title: 'Catat Transaksi',
    text: 'Input pemasukan & pengeluaran dengan antarmuka yang cepat.',
    icon: CreditCard,
  },
  {
    num: '03',
    title: 'Analisis & Kontrol',
    text: 'Gunakan insight dashboard untuk keputusan finansial yang lebih cerdas.',
    icon: TrendingUp,
  },
]

const transactions = [
  { name: 'Gaji Bulanan', amount: '+Rp 8.500.000', type: 'income', category: 'Salary' },
  { name: 'Grocery Store', amount: '-Rp 450.000', type: 'expense', category: 'Groceries' },
  { name: 'Netflix', amount: '-Rp 186.000', type: 'expense', category: 'Entertainment' },
  { name: 'Freelance Project', amount: '+Rp 2.500.000', type: 'income', category: 'Freelance' },
  { name: 'Listrik & Air', amount: '-Rp 670.000', type: 'expense', category: 'Utilities' },
]
</script>

<template>
  <div
    class="relative min-h-screen overflow-hidden bg-[#f8f9fc] text-slate-900 selection:bg-emerald-500/20 selection:text-emerald-900">

    <div class="pointer-events-none fixed inset-0 -z-10 overflow-hidden">
      <div
        class="landing-orb absolute -left-32 top-0 h-[700px] w-[700px] rounded-full bg-emerald-400/[0.08] blur-[150px]" />
      <div
        class="landing-orb-delayed absolute -right-48 top-[30%] h-[600px] w-[600px] rounded-full bg-sky-400/[0.06] blur-[130px]" />
      <div
        class="landing-orb absolute bottom-0 left-[40%] h-[500px] w-[500px] rounded-full bg-violet-400/[0.05] blur-[120px]" />

      <div class="absolute inset-0 opacity-40" style="
        background-image:
          radial-gradient(at 20% 30%, rgba(16,185,129,0.08) 0px, transparent 50%),
          radial-gradient(at 80% 20%, rgba(56,189,248,0.06) 0px, transparent 50%),
          radial-gradient(at 50% 80%, rgba(139,92,246,0.04) 0px, transparent 50%);
      " />

      <div class="absolute inset-0 opacity-[0.03]" style="
        background-image: radial-gradient(circle, rgba(0,0,0,0.15) 1px, transparent 1px);
        background-size: 32px 32px;
      " />
    </div>

    <header class="sticky top-0 z-50 border-b border-slate-200/60 bg-white/70 backdrop-blur-2xl">
      <div class="mx-auto flex max-w-7xl items-center justify-between px-6 py-3.5 lg:px-10">
        <NuxtLink to="/" class="group flex items-center gap-3">
          <div
            class="relative flex size-10 items-center justify-center rounded-xl bg-gradient-to-br from-emerald-500 via-emerald-600 to-teal-600 shadow-lg shadow-emerald-500/25 transition-all duration-300 group-hover:shadow-emerald-500/40 group-hover:scale-105">
            <PiggyBank class="size-5 text-white" />
            <div class="absolute inset-0 rounded-xl bg-white/20 opacity-0 transition-opacity group-hover:opacity-100" />
          </div>
          <div>
            <p class="text-sm font-bold tracking-wide text-slate-900">FinanceApp</p>
            <p class="text-[10px] font-medium uppercase tracking-[0.2em] text-slate-400">Money Tracker</p>
          </div>
        </NuxtLink>

        <nav
          class="hidden items-center gap-1 rounded-full border border-slate-200/80 bg-slate-50/80 px-2 py-1.5 md:flex">
          <a href="#fitur"
            class="rounded-full px-4 py-1.5 text-sm text-slate-500 transition-all hover:bg-white hover:text-slate-900 hover:shadow-sm">Fitur</a>
          <a href="#cara-kerja"
            class="rounded-full px-4 py-1.5 text-sm text-slate-500 transition-all hover:bg-white hover:text-slate-900 hover:shadow-sm">Cara
            Kerja</a>
          <a href="#preview"
            class="rounded-full px-4 py-1.5 text-sm text-slate-500 transition-all hover:bg-white hover:text-slate-900 hover:shadow-sm">Preview</a>
        </nav>

        <div class="flex items-center gap-3">
          <Button as-child variant="ghost"
            class="hidden rounded-full px-5 text-sm text-slate-600 hover:text-slate-900 md:inline-flex">
            <NuxtLink to="/login">Masuk</NuxtLink>
          </Button>
          <Button as-child
            class="group relative overflow-hidden rounded-full bg-gradient-to-r from-emerald-600 to-teal-600 px-6 py-2.5 text-sm font-medium text-white shadow-lg shadow-emerald-600/25 transition-all hover:shadow-emerald-600/40">
            <NuxtLink to="/register">
              Mulai Gratis
              <ArrowRight class="ml-1 inline size-3.5 transition-transform group-hover:translate-x-0.5" />
            </NuxtLink>
          </Button>
        </div>
      </div>
    </header>

    <section class="relative mx-auto max-w-7xl px-6 pt-20 lg:px-10 lg:pt-32">
      <div class="text-center"
        :class="isLoaded ? 'animate-in fade-in slide-in-from-bottom-6 duration-700' : 'opacity-0'">

        <h1
          class="mx-auto max-w-4xl text-5xl font-extrabold leading-[1.05] tracking-tight text-slate-900 sm:text-6xl lg:text-7xl">
          <span class="block">Kelola Uangmu</span>
          <span class="relative mt-2 block">
            <span class="bg-gradient-to-r from-emerald-600 via-teal-500 to-cyan-500 bg-clip-text text-transparent">
              Tanpa Ribet
            </span>
            <svg class="absolute -bottom-2 left-1/2 w-64 -translate-x-1/2 sm:w-80" viewBox="0 0 300 12" fill="none">
              <path d="M2 8C50 2 100 2 150 6C200 10 250 4 298 8" stroke="url(#underline-gradient)" stroke-width="3"
                stroke-linecap="round" />
              <defs>
                <linearGradient id="underline-gradient" x1="0" y1="0" x2="300" y2="0">
                  <stop offset="0%" stop-color="#10b981" stop-opacity="0.4" />
                  <stop offset="50%" stop-color="#14b8a6" />
                  <stop offset="100%" stop-color="#06b6d4" stop-opacity="0.4" />
                </linearGradient>
              </defs>
            </svg>
          </span>
        </h1>

        <p class="mx-auto mt-8 max-w-2xl text-lg leading-relaxed text-slate-500 sm:text-xl">
          Catat pemasukan, kontrol pengeluaran, dan bangun kebiasaan finansial sehat — semua dari satu dashboard yang
          beautiful & powerful.
        </p>

        <div class="mt-10 flex flex-col items-center justify-center gap-4 sm:flex-row">
          <Button as-child size="lg"
            class="group relative overflow-hidden rounded-full bg-gradient-to-r from-emerald-600 via-emerald-600 to-teal-600 px-10 py-6 text-base font-semibold text-white shadow-2xl shadow-emerald-600/25 transition-all duration-300 hover:shadow-emerald-600/40">
            <NuxtLink to="/register">
              Mulai Gratis Sekarang
              <ArrowUpRight
                class="ml-2 size-5 transition-transform group-hover:-translate-y-0.5 group-hover:translate-x-0.5" />
            </NuxtLink>
          </Button>
        </div>
      </div>

      <div id="preview" class="relative mt-20"
        :class="isLoaded ? 'animate-in fade-in slide-in-from-bottom-10 duration-1000 delay-300' : 'opacity-0'">
        <div class="absolute inset-x-10 -top-10 h-40 rounded-full bg-emerald-400/10 blur-[80px]" />
        <div class="absolute inset-x-20 -bottom-10 h-32 rounded-full bg-sky-400/8 blur-[60px]" />

        <div
          class="relative mx-auto max-w-5xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-[0_40px_120px_-20px_rgba(0,0,0,0.08)]">
          <div class="flex items-center justify-between border-b border-slate-100 px-5 py-3">
            <div class="flex items-center gap-3">
              <div class="flex gap-2">
                <div class="size-3 rounded-full bg-[#ff5f57]" />
                <div class="size-3 rounded-full bg-[#febc2e]" />
                <div class="size-3 rounded-full bg-[#28c840]" />
              </div>
              <div class="ml-3 flex items-center gap-2 rounded-lg bg-slate-50 px-3 py-1">
                <ShieldCheck class="size-3 text-emerald-500" />
                <span class="text-[11px] text-slate-400">financeapp.id/dashboard</span>
              </div>
            </div>
            <div class="flex items-center gap-2 rounded-full bg-emerald-50 px-3 py-1">
              <span class="size-1.5 rounded-full bg-emerald-500 animate-pulse" />
              <span class="text-[10px] font-medium text-emerald-600">LIVE PREVIEW</span>
            </div>
          </div>

          <div class="grid gap-4 bg-slate-50/50 p-5 lg:grid-cols-[1.2fr_0.8fr]">
            <div class="space-y-4">
              <div
                class="relative overflow-hidden rounded-xl bg-gradient-to-br from-emerald-600 via-emerald-700 to-teal-700 p-6 text-white">
                <div class="absolute right-0 top-0 h-40 w-40 rounded-full bg-white/10 blur-[50px]" />
                <div class="relative flex items-start justify-between">
                  <div>
                    <p
                      class="flex items-center gap-2 text-xs font-medium uppercase tracking-[0.2em] text-emerald-200/80">
                      <Wallet class="size-3.5" />
                      Total Saldo
                    </p>
                    <p class="mt-3 text-[2.5rem] font-bold leading-none tracking-tight">
                      Rp 12.<span class="text-emerald-200">450</span>.000
                    </p>
                    <p class="mt-2 flex items-center gap-1.5 text-sm text-emerald-100/70">
                      <TrendingUp class="size-3.5 text-emerald-200" />
                      <span class="text-emerald-200">+12.5%</span> dari bulan lalu
                    </p>
                  </div>
                  <div class="rounded-2xl bg-white/15 p-4">
                    <BadgeDollarSign class="size-8 text-white" />
                  </div>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4">
                <div class="rounded-xl border border-emerald-100 bg-white p-4">
                  <div class="flex items-center justify-between">
                    <span class="text-xs font-medium text-slate-500">Pemasukan</span>
                    <span
                      class="rounded-full bg-emerald-50 px-2 py-0.5 text-[10px] font-semibold text-emerald-600">+18%</span>
                  </div>
                  <p class="mt-3 text-2xl font-bold text-slate-900">Rp 8.2<span class="text-lg text-slate-400">jt</span>
                  </p>
                  <div class="mt-3 h-1.5 overflow-hidden rounded-full bg-emerald-100">
                    <div class="h-full w-[78%] rounded-full bg-gradient-to-r from-emerald-500 to-emerald-400" />
                  </div>
                </div>
                <div class="rounded-xl border border-amber-100 bg-white p-4">
                  <div class="flex items-center justify-between">
                    <span class="text-xs font-medium text-slate-500">Pengeluaran</span>
                    <span
                      class="rounded-full bg-amber-50 px-2 py-0.5 text-[10px] font-semibold text-amber-600">Aman</span>
                  </div>
                  <p class="mt-3 text-2xl font-bold text-slate-900">Rp 4.6<span class="text-lg text-slate-400">jt</span>
                  </p>
                  <div class="mt-3 h-1.5 overflow-hidden rounded-full bg-amber-100">
                    <div class="h-full w-[52%] rounded-full bg-gradient-to-r from-amber-500 to-orange-400" />
                  </div>
                </div>
              </div>

              <div class="rounded-xl border border-slate-100 bg-white p-5">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-xs font-medium text-slate-400">Cashflow 7 hari terakhir</p>
                    <p class="mt-0.5 text-sm font-semibold text-emerald-600">Trend Positif ↑</p>
                  </div>
                  <ChartColumnBig class="size-5 text-emerald-400" />
                </div>
                <div class="mt-5 flex h-24 items-end gap-1.5">
                  <div v-for="(h, i) in [40, 65, 50, 80, 55, 92, 70]" :key="i"
                    class="relative flex-1 overflow-hidden rounded-t-lg transition-all duration-500 hover:opacity-100"
                    :style="{ height: h + '%' }">
                    <div class="absolute inset-0 rounded-t-lg"
                      :class="i === 5 ? 'bg-gradient-to-t from-emerald-500 to-emerald-300 shadow-lg shadow-emerald-500/20' : i % 3 === 2 ? 'bg-gradient-to-t from-amber-400 to-amber-200' : 'bg-gradient-to-t from-emerald-400 to-emerald-200'" />
                    <div v-if="i === 5"
                      class="absolute -top-6 left-1/2 -translate-x-1/2 whitespace-nowrap rounded-md bg-emerald-500 px-1.5 py-0.5 text-[9px] font-semibold text-white">
                      Tertinggi</div>
                  </div>
                </div>
                <div class="mt-2 flex justify-between text-[9px] text-slate-400">
                  <span>Sen</span><span>Sel</span><span>Rab</span><span>Kam</span><span>Jum</span><span>Sab</span><span>Min</span>
                </div>
              </div>
            </div>

            <div class="space-y-4">
              <div class="rounded-xl border border-violet-100 bg-gradient-to-br from-violet-50 to-white p-5">
                <div class="flex items-center justify-between">
                  <p class="flex items-center gap-2 text-xs font-medium text-violet-600">
                    <PiggyBank class="size-3.5" />
                    Saving Target
                  </p>
                  <span class="text-[10px] text-violet-400">68% reached</span>
                </div>
                <p class="mt-3 text-2xl font-bold text-slate-900">Rp 2.5<span class="text-lg text-slate-400">jt</span>
                </p>
                <div class="mt-3 h-2 overflow-hidden rounded-full bg-violet-100">
                  <div class="h-full w-[68%] rounded-full bg-gradient-to-r from-violet-500 to-purple-400" />
                </div>
                <p class="mt-2 text-[11px] text-slate-400">Target: Rp 3.750.000 / bulan</p>
              </div>

              <div class="rounded-xl border border-slate-100 bg-white p-4">
                <div class="flex items-center justify-between">
                  <p class="text-xs font-semibold text-slate-500">Transaksi Terakhir</p>
                  <Receipt class="size-3.5 text-slate-300" />
                </div>
                <div class="mt-3 space-y-1">
                  <div v-for="tx in transactions" :key="tx.name"
                    class="flex items-center justify-between rounded-lg px-2 py-2 transition-colors hover:bg-slate-50">
                    <div class="flex items-center gap-2.5">
                      <div class="flex size-7 items-center justify-center rounded-lg"
                        :class="tx.type === 'income' ? 'bg-emerald-50' : 'bg-rose-50'">
                        <TrendingUp v-if="tx.type === 'income'" class="size-3 text-emerald-500" />
                        <Send v-else class="size-3 rotate-180 text-rose-500" />
                      </div>
                      <div>
                        <p class="text-xs font-medium text-slate-700">{{ tx.name }}</p>
                        <p class="text-[10px] text-slate-400">{{ tx.category }}</p>
                      </div>
                    </div>
                    <p class="text-xs font-semibold"
                      :class="tx.type === 'income' ? 'text-emerald-600' : 'text-rose-500'">{{ tx.amount }}</p>
                  </div>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div class="rounded-xl border border-slate-100 bg-white p-3.5">
                  <BanknoteArrowDown class="size-4 text-amber-500" />
                  <p class="mt-2 text-lg font-bold text-slate-900">76%</p>
                  <p class="text-[10px] text-slate-400">Budget Groceries</p>
                </div>
                <div class="rounded-xl border border-slate-100 bg-white p-3.5">
                  <ChartPie class="size-4 text-cyan-500" />
                  <p class="mt-2 text-lg font-bold text-slate-900">5</p>
                  <p class="text-[10px] text-slate-400">Wallet Aktif</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section id="fitur" class="relative mx-auto max-w-7xl px-6 py-32 lg:px-10">
      <div class="mb-20 flex flex-col items-center text-center">
        <div
          class="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-emerald-50 px-4 py-1.5 text-xs font-semibold uppercase tracking-widest text-emerald-700">
          <Zap class="size-3" />
          Fitur Unggulan
        </div>
        <h2 class="mt-6 text-3xl font-bold leading-tight text-slate-900 sm:text-4xl lg:text-5xl">
          Semua tools untuk<br />
          <span
            class="bg-gradient-to-r from-emerald-600 via-teal-500 to-cyan-500 bg-clip-text text-transparent">financial
            freedom</span>
        </h2>
        <p class="mt-5 max-w-xl text-base leading-relaxed text-slate-500">
          Dari pencatatan harian hingga analisis bulanan — kontrol penuh atas setiap rupiah.
        </p>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <article v-for="(f, i) in features" :key="f.title"
          class="group relative overflow-hidden rounded-2xl border border-slate-200/80 bg-white p-7 shadow-sm transition-all duration-500 hover:border-slate-200 hover:shadow-lg hover:shadow-slate-200/50">
          <div
            class="pointer-events-none absolute -right-10 -top-10 h-32 w-32 rounded-full opacity-0 blur-[50px] transition-opacity duration-500 group-hover:opacity-100"
            :class="{
              'bg-emerald-400/30': f.accent === 'emerald',
              'bg-cyan-400/30': f.accent === 'cyan',
              'bg-violet-400/30': f.accent === 'violet',
              'bg-amber-400/30': f.accent === 'amber',
              'bg-rose-400/30': f.accent === 'rose',
              'bg-sky-400/30': f.accent === 'sky',
            }" />

          <div
            class="flex size-12 items-center justify-center rounded-xl transition-all duration-300 group-hover:scale-110 group-hover:shadow-lg"
            :class="{
              'bg-emerald-50 text-emerald-600 group-hover:shadow-emerald-200/50': f.accent === 'emerald',
              'bg-cyan-50 text-cyan-600 group-hover:shadow-cyan-200/50': f.accent === 'cyan',
              'bg-violet-50 text-violet-600 group-hover:shadow-violet-200/50': f.accent === 'violet',
              'bg-amber-50 text-amber-600 group-hover:shadow-amber-200/50': f.accent === 'amber',
              'bg-rose-50 text-rose-600 group-hover:shadow-rose-200/50': f.accent === 'rose',
              'bg-sky-50 text-sky-600 group-hover:shadow-sky-200/50': f.accent === 'sky',
            }">
            <component :is="f.icon" class="size-5" />
          </div>
          <h3 class="mt-5 text-lg font-semibold text-slate-900">{{ f.title }}</h3>
          <p class="mt-2 text-sm leading-relaxed text-slate-500">{{ f.description }}</p>
        </article>
      </div>
    </section>

    <section id="cara-kerja" class="mx-auto max-w-7xl px-6 py-20 lg:px-10">
      <div class="mb-20 flex flex-col items-center text-center">
        <div
          class="inline-flex items-center gap-2 rounded-full border border-cyan-200 bg-cyan-50 px-4 py-1.5 text-xs font-semibold uppercase tracking-widest text-cyan-700">
          <Sparkles class="size-3" />
          Cara Kerja
        </div>
        <h2 class="mt-6 text-3xl font-bold text-slate-900 sm:text-4xl lg:text-5xl">
          3 langkah menuju<br />
          <span class="bg-gradient-to-r from-cyan-500 to-emerald-500 bg-clip-text text-transparent">keuangan
            terkontrol</span>
        </h2>
      </div>

      <div class="relative grid gap-6 md:grid-cols-3">
        <div class="pointer-events-none absolute left-0 right-0 top-[4.5rem] hidden h-px md:block">
          <div class="h-full w-full bg-gradient-to-r from-transparent via-emerald-300 to-transparent" />
        </div>

        <article v-for="step in steps" :key="step.num" class="group relative flex flex-col items-center text-center">
          <div class="relative mb-8">
            <div
              class="relative flex size-20 items-center justify-center rounded-3xl border border-slate-200 bg-white shadow-sm transition-all duration-500 group-hover:border-emerald-200 group-hover:shadow-xl group-hover:shadow-emerald-100/80">
              <component :is="step.icon"
                class="size-7 text-emerald-600 transition-transform duration-500 group-hover:scale-110" />
            </div>
            <div
              class="absolute -right-2 -top-2 flex size-7 items-center justify-center rounded-full bg-gradient-to-br from-emerald-500 to-teal-500 text-[10px] font-bold text-white shadow-lg shadow-emerald-500/25">
              {{ step.num }}
            </div>
          </div>

          <h3 class="text-xl font-bold text-slate-900">{{ step.title }}</h3>
          <p class="mt-3 max-w-xs text-sm leading-relaxed text-slate-500">{{ step.text }}</p>
        </article>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-6 py-20 lg:px-10">
      <div class="relative overflow-hidden rounded-3xl border border-slate-200">
        <div class="absolute inset-0 bg-gradient-to-br from-emerald-600 via-emerald-700 to-teal-700" />
        <div class="pointer-events-none absolute -left-20 top-0 h-60 w-60 rounded-full bg-white/10 blur-[100px]" />
        <div
          class="pointer-events-none absolute -right-20 bottom-0 h-60 w-60 rounded-full bg-teal-300/15 blur-[100px]" />

        <div class="relative px-8 py-20 text-center text-white lg:px-16">
          <div
            class="mx-auto mb-8 flex size-20 items-center justify-center rounded-3xl bg-white/15 shadow-2xl shadow-black/10 backdrop-blur-sm">
            <CircleDollarSign class="size-10 text-white" />
          </div>

          <h2 class="text-3xl font-bold sm:text-4xl lg:text-5xl">
            Siap kelola keuanganmu<br class="hidden sm:block" />
            <span class="text-emerald-200">lebih
              cerdas?</span>
          </h2>
          <p class="mx-auto mt-5 max-w-xl text-base text-emerald-100/70">
            Bergabung sekarang — gratis, tanpa kartu kredit, langsung bisa dipakai.
          </p>

          <div class="mt-10 flex flex-col items-center justify-center gap-4 sm:flex-row">
            <Button as-child size="lg"
              class="rounded-full bg-white px-10 py-6 text-base font-semibold text-emerald-700 shadow-2xl transition-all hover:bg-emerald-50 hover:shadow-white/20">
              <NuxtLink to="/register">
                Buat Akun Gratis
                <ArrowUpRight class="ml-2 size-5" />
              </NuxtLink>
            </Button>
            <Button as-child variant="ghost" size="lg"
              class="rounded-full border border-white/20 px-8 py-6 text-base text-white hover:bg-white/10">
              <NuxtLink to="/login">Masuk ke Dashboard</NuxtLink>
            </Button>
          </div>
        </div>
      </div>
    </section>

    <footer class="border-t border-slate-200">
      <div class="mx-auto flex max-w-7xl flex-col items-center justify-between gap-6 px-6 py-10 sm:flex-row lg:px-10">
        <div class="flex items-center gap-3">
          <div
            class="flex size-8 items-center justify-center rounded-lg bg-gradient-to-br from-emerald-500 to-teal-600">
            <PiggyBank class="size-4 text-white" />
          </div>
          <span class="text-sm font-bold text-slate-400">FinanceApp</span>
        </div>
        <p class="text-xs text-slate-400">&copy; {{ new Date().getFullYear() }} FinanceApp. All rights reserved.</p>
        <div class="flex items-center gap-8 text-xs text-slate-400">
          <a href="#" class="transition-colors hover:text-slate-600">Privacy</a>
          <a href="#" class="transition-colors hover:text-slate-600">Terms</a>
          <a href="#" class="transition-colors hover:text-slate-600">Contact</a>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
@keyframes float-orb {

  0%,
  100% {
    transform: translate(0, 0) scale(1);
  }

  33% {
    transform: translate(30px, -20px) scale(1.05);
  }

  66% {
    transform: translate(-20px, 15px) scale(0.95);
  }
}

@keyframes float-orb-delayed {

  0%,
  100% {
    transform: translate(0, 0) scale(1);
  }

  33% {
    transform: translate(-25px, 20px) scale(0.95);
  }

  66% {
    transform: translate(20px, -15px) scale(1.05);
  }
}

.landing-orb {
  animation: float-orb 15s ease-in-out infinite;
}

.landing-orb-delayed {
  animation: float-orb-delayed 18s ease-in-out infinite;
}
</style>
