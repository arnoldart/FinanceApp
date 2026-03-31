<script setup lang="ts">
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import Button from '~/components/ui/button/Button.vue'
import MoneyConverter from '~/lib/MoneyConverter'
import type { Wallet } from '~/types/wallet'

const props = defineProps<{
    transactionId?: string
}>()

const { $api } = useNuxtApp()
const router = useRouter()

const isEditMode = computed(() => !!props.transactionId)

const form = reactive({
    walletId: '',
    type: 1,
    amount: '',
    note: '',
})

const isLoading = ref(true)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const wallets = ref<{ walletId: string; walletName: string }[]>([])

onMounted(async () => {
    try {
        const walletsRes = await $api('/api/wallet', { method: 'GET' }) as Wallet[]
        wallets.value = walletsRes.map((w) => ({
            walletId: w.id,
            walletName: w.name,
        }))

        if (!isEditMode.value && wallets.value.length > 0) {
            form.walletId = wallets.value[0].walletId
        }

        if (isEditMode.value) {
            const txRes = await $api(`/api/transaction/${props.transactionId}`, { method: 'GET' }) as any
            const tx = Array.isArray(txRes) ? txRes[0] : txRes

            if (tx) {
                if (tx.walletId) {
                    form.walletId = tx.walletId
                } else if (tx.walletName) {
                    const match = wallets.value.find(w => w.walletName === tx.walletName)
                    if (match) form.walletId = match.walletId
                }

                form.type = tx.type
                form.amount = String(tx.amount)
                form.note = tx.note || ''
            }
        }
    } catch (e) {
        console.error('Failed to load data', e)
    } finally {
        isLoading.value = false
    }
})

const displayAmount = computed(() => {
    if (!form.amount) return ''
    return MoneyConverter(Number(form.amount))
})

function onAmountInput(e: Event) {
    const target = e.target as HTMLInputElement
    const raw = target.value.replace(/\D/g, '')
    form.amount = raw
}

function validate(): boolean {
    errors.value = {}

    if (!form.walletId) errors.value.walletId = 'Pilih rekening terlebih dahulu'
    if (!form.amount || Number(form.amount) <= 0) errors.value.amount = 'Jumlah harus lebih dari 0'

    return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
    if (!validate()) return

    isSubmitting.value = true

    try {
        if (isEditMode.value) {
            await $api(`/api/transaction/${props.transactionId}`, {
                method: 'PATCH',
                body: {
                    walletId: form.walletId,
                    type: form.type,
                    amount: Number(form.amount),
                    note: form.note || null,
                },
            })
        } else {
            await $api('/api/transaction', {
                method: 'POST',
                body: {
                    walletId: form.walletId,
                    type: form.type,
                    amount: Number(form.amount),
                    note: form.note || null,
                },
            })
        }

        await router.push('/transaction')
    } catch (e) {
        console.error('Failed to save transaction', e)
    } finally {
        isSubmitting.value = false
    }
}
</script>

<template>
    <div class="mb-8">
        <NuxtLink to="/transaction"
            class="mb-4 inline-flex items-center gap-1.5 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none"
                stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="15 18 9 12 15 6" />
            </svg>
            Kembali ke Transaksi
        </NuxtLink>
        <h1 class="text-2xl font-bold tracking-tight text-foreground">
            {{ isEditMode ? 'Edit Transaksi' : 'Tambah Transaksi' }}
        </h1>
        <p class="mt-1 text-sm text-muted-foreground">
            {{ isEditMode ? 'Ubah rincian transaksi yang sudah ada' : 'Catat transaksi baru ke dalam rekening kamu' }}
        </p>
    </div>

    <div>
        <Card class="overflow-hidden rounded-2xl border-border/60 shadow-sm relative">
            <div v-if="isLoading" class="absolute inset-0 z-10 flex items-center justify-center bg-background/80 backdrop-blur-sm">
                <span class="text-sm font-medium text-muted-foreground animate-pulse">Memuat data...</span>
            </div>

            <CardHeader class="border-b border-border/40 pb-6">
                <CardTitle class="text-lg font-semibold">Detail Transaksi</CardTitle>
                <CardDescription>
                    {{ isEditMode ? 'Ubah informasi di bawah ini' : 'Isi informasi di bawah ini untuk mencatat transaksi baru' }}
                </CardDescription>
            </CardHeader>

            <CardContent class="space-y-6 pt-6">
                <form @submit.prevent="handleSubmit" class="space-y-6">

                    <div class="space-y-2">
                        <Label class="text-sm font-medium">Tipe Transaksi</Label>
                        <div class="grid grid-cols-2 gap-3">
                            <button type="button" @click="form.type = 1"
                                class="group relative flex items-center justify-center gap-2.5 rounded-xl border-2 px-4 py-3.5 text-sm font-semibold transition-all duration-200"
                                :class="form.type === 1
                                    ? 'border-rose-500/50 bg-rose-500/8 text-rose-700 shadow-sm'
                                    : 'border-border/60 text-muted-foreground hover:border-border hover:text-foreground'">
                                <div class="flex h-8 w-8 items-center justify-center rounded-lg"
                                    :class="form.type === 1 ? 'bg-rose-500/15' : 'bg-muted/50'">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4"
                                        :class="form.type === 1 ? 'text-rose-600' : 'text-muted-foreground'"
                                        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                        <line x1="12" y1="5" x2="12" y2="19" />
                                        <polyline points="19 12 12 19 5 12" />
                                    </svg>
                                </div>
                                Pengeluaran
                            </button>

                            <button type="button" @click="form.type = 0"
                                class="group relative flex items-center justify-center gap-2.5 rounded-xl border-2 px-4 py-3.5 text-sm font-semibold transition-all duration-200"
                                :class="form.type === 0
                                    ? 'border-emerald-500/50 bg-emerald-500/8 text-emerald-700 shadow-sm'
                                    : 'border-border/60 text-muted-foreground hover:border-border hover:text-foreground'">
                                <div class="flex h-8 w-8 items-center justify-center rounded-lg"
                                    :class="form.type === 0 ? 'bg-emerald-500/15' : 'bg-muted/50'">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4"
                                        :class="form.type === 0 ? 'text-emerald-600' : 'text-muted-foreground'"
                                        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                        <line x1="12" y1="19" x2="12" y2="5" />
                                        <polyline points="5 12 12 5 19 12" />
                                    </svg>
                                </div>
                                Pemasukan
                            </button>
                        </div>
                    </div>

                    <div class="space-y-2">
                        <Label for="wallet" class="text-sm font-medium">Rekening</Label>
                        <div class="relative">
                            <select id="wallet" v-model="form.walletId"
                                class="h-11 w-full appearance-none rounded-xl border bg-transparent px-4 pr-10 text-sm font-medium outline-none transition-all duration-200"
                                :class="errors.walletId
                                    ? 'border-destructive ring-destructive/20 ring-[3px]'
                                    : 'border-border/60 focus:border-ring focus:ring-ring/50 focus:ring-[3px]'">
                                <option value="" disabled>Pilih rekening...</option>
                                <option v-for="wallet in wallets" :key="wallet.walletId" :value="wallet.walletId">
                                    {{ wallet.walletName }}
                                </option>
                            </select>
                            <svg xmlns="http://www.w3.org/2000/svg"
                                class="pointer-events-none absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
                                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                                stroke-linecap="round" stroke-linejoin="round">
                                <polyline points="6 9 12 15 18 9" />
                            </svg>
                        </div>
                        <p v-if="errors.walletId" class="text-xs text-destructive">{{ errors.walletId }}</p>
                    </div>

                    <div class="space-y-2">
                        <Label for="amount" class="text-sm font-medium">Jumlah</Label>
                        <div class="relative">
                            <span
                                class="absolute left-4 top-1/2 -translate-y-1/2 text-sm font-semibold text-muted-foreground">Rp</span>
                            <input id="amount" type="text" inputmode="numeric" placeholder="0" :value="displayAmount"
                                @input="onAmountInput"
                                class="h-12 w-full rounded-xl border bg-transparent pl-10 pr-4 text-lg font-bold tracking-tight outline-none transition-all duration-200"
                                :class="errors.amount
                                    ? 'border-destructive ring-destructive/20 ring-[3px]'
                                    : 'border-border/60 focus:border-ring focus:ring-ring/50 focus:ring-[3px]'" />
                        </div>
                        <p v-if="errors.amount" class="text-xs text-destructive">{{ errors.amount }}</p>
                    </div>

                    <div class="space-y-2">
                        <Label for="note" class="text-sm font-medium">
                            Keterangan
                            <span class="font-normal text-muted-foreground">(opsional)</span>
                        </Label>
                        <textarea id="note" v-model="form.note" rows="3"
                            placeholder="Contoh: Bayar makan siang, gaji bulanan..."
                            class="w-full resize-none rounded-xl border border-border/60 bg-transparent px-4 py-3 text-sm outline-none transition-all duration-200 placeholder:text-muted-foreground focus:border-ring focus:ring-ring/50 focus:ring-[3px]" />
                    </div>

                    <div class="border-t border-border/40" />

                    <div v-if="form.amount" class="rounded-xl border border-border/50 bg-muted/20 p-4">
                        <p class="mb-2 text-xs font-semibold uppercase tracking-[0.2em] text-muted-foreground">Preview
                        </p>
                        <div class="flex items-center justify-between">
                            <div class="flex items-center gap-3">
                                <div class="flex h-10 w-10 items-center justify-center rounded-xl"
                                    :class="form.type === 1 ? 'bg-rose-500/10' : 'bg-emerald-500/10'">
                                    <svg v-if="form.type === 1" xmlns="http://www.w3.org/2000/svg"
                                        class="h-4 w-4 text-rose-600" viewBox="0 0 24 24" fill="none"
                                        stroke="currentColor" stroke-width="2" stroke-linecap="round"
                                        stroke-linejoin="round">
                                        <line x1="12" y1="5" x2="12" y2="19" />
                                        <polyline points="19 12 12 19 5 12" />
                                    </svg>
                                    <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-emerald-600"
                                        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                        <line x1="12" y1="19" x2="12" y2="5" />
                                        <polyline points="5 12 12 5 19 12" />
                                    </svg>
                                </div>
                                <div>
                                    <p class="text-sm font-medium text-foreground">{{ form.note || 'Tanpa keterangan' }}
                                    </p>
                                    <p class="text-xs text-muted-foreground">
                                        {{wallets.find(w => w.walletId === form.walletId)?.walletName || '-'}}
                                        | {{ form.type === 1 ? 'Pengeluaran' : 'Pemasukan' }}
                                    </p>
                                </div>
                            </div>
                            <p class="text-lg font-bold"
                                :class="form.type === 1 ? 'text-rose-600' : 'text-emerald-600'">
                                {{ form.type === 1 ? '-' : '+' }} Rp {{ displayAmount || '0' }}
                            </p>
                        </div>
                    </div>

                    <div class="flex gap-3 pt-2">
                        <NuxtLink to="/transaction" class="flex-1">
                            <Button type="button" variant="outline" class="w-full rounded-xl py-5 text-sm">
                                Batal
                            </Button>
                        </NuxtLink>
                        <Button type="submit" class="flex-1 rounded-xl py-5 text-sm" :disabled="isSubmitting || isLoading">
                            <svg v-if="isSubmitting" xmlns="http://www.w3.org/2000/svg"
                                class="mr-2 h-4 w-4 animate-spin" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2">
                                <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                            </svg>
                            {{ isSubmitting ? 'Menyimpan...' : 'Simpan Transaksi' }}
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    </div>
</template>

