namespace FinanceApp.API.DTOs.Wallet;

public class UpdateWalletDto
{
    public string Name {get; set;} = string.Empty;
    public decimal Balance { get; set; }
}
