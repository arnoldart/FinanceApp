namespace FinanceApp.API.Models;

public class Wallet : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User {get; set;} = null!;

    public string Name { get; set; } = null!;

    public decimal Balance { get; set; }

    public ICollection<Transaction> Transactions {get; set;} = new List<Transaction>();
}