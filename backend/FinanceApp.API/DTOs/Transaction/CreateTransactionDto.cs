using FinanceApp.API.Models;

namespace FinanceApp.API.DTOs.Transaction;

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Note { get; set; }
}
