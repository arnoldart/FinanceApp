namespace FinanceApp.API.Models;

public class Transaction : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid WalletId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; }

    public string Note { get; set; }

    public DateTime TransactionDate { get; set; }
}