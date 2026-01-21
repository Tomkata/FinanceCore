namespace BankingSystem.Application.DTOs.Auth
{
    /// <summary>
    /// Result of user login containing user info and JWT token.
    /// </summary>
    public class LoginResultDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
    }
}
