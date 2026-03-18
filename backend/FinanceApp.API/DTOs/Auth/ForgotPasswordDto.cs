using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.DTOs.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
