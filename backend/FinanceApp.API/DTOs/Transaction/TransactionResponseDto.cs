using FinanceApp.API.Models;

namespace FinanceApp.API.DTOs.Transaction;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    // public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
