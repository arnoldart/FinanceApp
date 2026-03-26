export interface WalletSummary {
  walletId: string
  walletName: string
  balance: number
  transactionCount: number
  incomeThisMonth: number
  expenseThisMonth: number
}

export interface RecentTransaction {
  id: string
  amount: number
  type: number
  note: string | null
}

export interface DashboardResponse {
  totalBalance: number
  totalIncomeThisMonth: number
  totalExpenseThisMonth: number
  recentTransactions: RecentTransaction[]
  walletSummaries: WalletSummary[]
}
