export interface Transaction {
  id: string
  walletName: string
  amount: number
  type: number
  note: string | null
  createdAt: string
  updatedAt: string
}
