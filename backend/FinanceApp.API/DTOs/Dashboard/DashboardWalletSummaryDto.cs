namespace FinanceApp.API.DTOs.Dashboard;

public class DashboardWalletSummaryDto
{
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int TransactionCount { get; set; }
    public decimal IncomeThisMonth { get; set; }
    public decimal ExpenseThisMonth { get; set; }
}
