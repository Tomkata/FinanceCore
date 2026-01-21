using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Infrastructure.Identity
{
    /// <summary>
    /// Application user for authentication and authorization.
    /// Extends IdentityUser to add custom properties.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Link to Customer aggregate if this user is a customer
        public Guid? CustomerId { get; set; }
    }
}
