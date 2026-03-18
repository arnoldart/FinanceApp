namespace FinanceApp.API.Models;

public class RefreshToken : BaseEntity
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public Guid DeviceId {get; set;}
    public string Token {get; set;}
    public DateTime ExpiresAt {get; set;}
    public bool Revoked {get; set;}
}