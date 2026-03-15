namespace FinanceApp.API.Models
{
    public class User : BaseEntity
    {
         public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public int TokenVersion { get; set; } = 0;
    }
}
