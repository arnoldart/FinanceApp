namespace FinanceApp.API.Models;

public class ForgotPassword : BaseEntity
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}

    public DateTime ExpiresAt {get; set;}
    public bool Used {get; set;}
}