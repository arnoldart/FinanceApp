using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.DTOs.Auth;

public class VerifyEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Token { get; set; }
}
