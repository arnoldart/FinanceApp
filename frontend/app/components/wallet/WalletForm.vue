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

const props = defineProps<{
    walletId?: string
}>()

const { $api } = useNuxtApp()
const router = useRouter()

const isEditMode = computed(() => !!props.walletId)

// Form state
const form = reactive({
    name: '',
    balance: '',
})

const isLoading = ref(isEditMode.value)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

onMounted(async () => {
    if (isEditMode.value) {
        try {
            const res = await $api(`/api/wallet/${props.walletId}`, { method: 'GET' }) as any
            const wallet = Array.isArray(res) ? res[0] : res

            if (wallet) {
                form.name = wallet.name || wallet.walletName // Handle different possible keys from API
                form.balance = wallet.balance != null ? String(wallet.balance) : '0'
            }
        } catch (e) {
            console.error('Failed to load wallet data', e)
        } finally {
            isLoading.value = false
        }
    }
})

// Balance display formatting
const displayBalance = computed(() => {
    if (!form.balance) return ''
    return MoneyConverter(Number(form.balance))
})

function onBalanceInput(e: Event) {
    const target = e.target as HTMLInputElement
    const raw = target.value.replace(/\D/g, '')
    form.balance = raw
}

function validate(): boolean {
    errors.value = {}

    if (!form.name.trim()) errors.value.name = 'Nama rekening tidak boleh kosong'
    if (form.name.length > 50) errors.value.name = 'Nama rekening maksimal 50 karakter'

    // Balance can be 0, but cannot be empty strictly if we require it.
    // Usually starting balance can be 0.
    if (!form.balance && form.balance !== '0') errors.value.balance = 'Saldo awal harus diisi'

    return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
    if (!validate()) return

    isSubmitting.value = true

    try {
        if (isEditMode.value) {
            await $api(`/api/wallet/${props.walletId}`, {
                method: 'PATCH',
                body: {
                    name: form.name.trim(),
                    balance: Number(form.balance || 0),
                },
            })
        } else {
            await $api('/api/wallet', {
                method: 'POST',
                body: {
                    name: form.name.trim(),
                    balance: Number(form.balance || 0),
                },
            })
        }

        await router.push('/wallet')
    } catch (e) {
        console.error('Failed to save wallet', e)
    } finally {
        isSubmitting.value = false
    }
}
</script>

<template>
    <div class="mb-8">
        <NuxtLink to="/wallet"
            class="mb-4 inline-flex items-center gap-1.5 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 24 24" fill="none"
                stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="15 18 9 12 15 6" />
            </svg>
            Kembali ke Rekening
        </NuxtLink>
        <h1 class="text-2xl font-bold tracking-tight text-foreground">
            {{ isEditMode ? 'Edit Rekening' : 'Tambah Rekening' }}
        </h1>
        <p class="mt-1 text-sm text-muted-foreground">
            {{ isEditMode ? 'Ubah rincian rekening yang sudah ada' : `Tambahkan rekening baru untuk mengelola keuangan
            kamu` }}
        </p>
    </div>

    <div>
        <Card class="overflow-hidden rounded-2xl border-border/60 shadow-sm relative">
            <div v-if="isLoading"
                class="absolute inset-0 z-10 flex items-center justify-center bg-background/80 backdrop-blur-sm">
                <span class="text-sm font-medium text-muted-foreground animate-pulse">Memuat data...</span>
            </div>

            <CardHeader class="border-b border-border/40 pb-6">
                <CardTitle class="text-lg font-semibold">Detail Rekening</CardTitle>
                <CardDescription>
                    {{ isEditMode ? 'Ubah informasi di bawah ini' : 'Isi nama rekening dan saldo awal' }}
                </CardDescription>
            </CardHeader>

            <CardContent class="space-y-6 pt-6">
                <form @submit.prevent="handleSubmit" class="space-y-6">

                    <div class="space-y-2">
                        <Label for="name" class="text-sm font-medium">Nama Rekening</Label>
                        <input id="name" v-model="form.name" type="text" placeholder="Contoh: BCA Debit, Cash, Gopay..."
                            class="h-11 w-full rounded-xl border bg-transparent px-4 text-sm font-medium outline-none transition-all duration-200 placeholder:font-normal placeholder:text-muted-foreground"
                            :class="errors.name
                                ? 'border-destructive ring-destructive/20 ring-[3px]'
                                : 'border-border/60 focus:border-ring focus:ring-ring/50 focus:ring-[3px]'" />
                        <p v-if="errors.name" class="text-xs text-destructive">{{ errors.name }}</p>
                    </div>

                    <div class="space-y-2">
                        <Label for="balance" class="text-sm font-medium">Saldo Awal</Label>
                        <div class="relative">
                            <span
                                class="absolute left-4 top-1/2 -translate-y-1/2 text-sm font-semibold text-muted-foreground">Rp</span>
                            <input id="balance" type="text" inputmode="numeric" placeholder="0" :value="displayBalance"
                                @input="onBalanceInput"
                                class="h-12 w-full rounded-xl border bg-transparent pl-10 pr-4 text-lg font-bold tracking-tight outline-none transition-all duration-200"
                                :class="errors.balance
                                    ? 'border-destructive ring-destructive/20 ring-[3px]'
                                    : 'border-border/60 focus:border-ring focus:ring-ring/50 focus:ring-[3px]'" />
                        </div>
                        <p v-if="errors.balance" class="text-xs text-destructive">{{ errors.balance }}</p>
                    </div>

                    <div class="border-t border-border/40" />

                    <div v-if="form.name || form.balance" class="rounded-xl border border-border/50 bg-muted/20 p-4">
                        <p class="mb-2 text-xs font-semibold uppercase tracking-[0.2em] text-muted-foreground">Preview
                        </p>
                        <div class="flex items-center justify-between">
                            <div class="flex items-center gap-3">
                                <div class="flex h-11 w-11 items-center justify-center rounded-xl bg-sky-500/10">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-sky-600"
                                        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M21 12V7H5a2 2 0 0 1 0-4h14v4" />
                                        <path d="M3 5v14a2 2 0 0 0 2 2h16v-5" />
                                        <path d="M18 12a2 2 0 0 0 0 4h4v-4Z" />
                                    </svg>
                                </div>
                                <div>
                                    <p class="text-sm font-semibold text-foreground">{{ form.name || 'Nama Rekening' }}
                                    </p>
                                    <p class="text-xs text-muted-foreground">Saldo Aktif</p>
                                </div>
                            </div>
                            <p class="text-lg font-bold text-foreground">
                                Rp {{ displayBalance || '0' }}
                            </p>
                        </div>
                    </div>

                    <div class="flex gap-3 pt-2">
                        <NuxtLink to="/wallet" class="flex-1">
                            <Button type="button" variant="outline" class="w-full rounded-xl py-5 text-sm">
                                Batal
                            </Button>
                        </NuxtLink>
                        <Button type="submit" class="flex-1 rounded-xl py-5 text-sm"
                            :disabled="isSubmitting || isLoading">
                            <svg v-if="isSubmitting" xmlns="http://www.w3.org/2000/svg"
                                class="mr-2 h-4 w-4 animate-spin" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2">
                                <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                            </svg>
                            {{ isSubmitting ? 'Menyimpan...' : 'Simpan Rekening' }}
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    </div>
</template>
