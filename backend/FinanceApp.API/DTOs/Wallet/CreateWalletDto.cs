namespace FinanceApp.API.DTOs.Wallet;

public class CreateWalletDto
{
    public string Name {get; set;} = string.Empty;
    public decimal Balance { get; set; }
}
