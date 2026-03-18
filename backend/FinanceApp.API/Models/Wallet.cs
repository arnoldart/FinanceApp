namespace FinanceApp.API.Models;

public class Wallet : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }
}