using FinanceApp.API.Models;

namespace FinanceApp.API.DTOs.Dashboard;

public class DashboardRecentTransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
