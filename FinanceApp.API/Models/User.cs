namespace FinanceApp.API.Models
{
    public class User : BaseEntity
    {
         public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public int TokenVersion { get; set; } = 0;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime? EmailVerifiedAt { get; set; }

        public string? EmailVerificationCode { get; set; }

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationExpiresAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }
        public DateTime? PasswordResetRequestedAt { get; set; }

    }
}
