namespace FinanceApp.API.Models;

public class Transaction : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public string? Note { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}