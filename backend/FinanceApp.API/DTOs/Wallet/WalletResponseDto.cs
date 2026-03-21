namespace FinanceApp.API.DTOs.Wallet;

public class WalletResponseDto
{
    public Guid Id { get; internal set; }
    public string Name {get; set;} = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
