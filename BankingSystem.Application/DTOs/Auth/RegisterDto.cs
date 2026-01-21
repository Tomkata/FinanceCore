namespace BankingSystem.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for user registration.
    /// </summary>
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Link this user to an existing customer
        /// </summary>
        public Guid? CustomerId { get; set; }
    }
}
