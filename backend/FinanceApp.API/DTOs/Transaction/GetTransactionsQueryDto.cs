using FinanceApp.API.Models;

namespace FinanceApp.API.DTOs.Transaction;

public class GetTransactionsQueryDto
{
    public Guid? WalletId { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
