namespace FinanceApp.API.DTOs.Dashboard;

public class DashboardResponseDto
{
    public decimal TotalBalance { get; set; }
    public decimal TotalIncomeThisMonth { get; set; }
    public decimal TotalExpenseThisMonth { get; set; }
    public List<DashboardRecentTransactionDto> RecentTransactions { get; set; } = [];
    public List<DashboardWalletSummaryDto> WalletSummaries { get; set; } = [];
}
