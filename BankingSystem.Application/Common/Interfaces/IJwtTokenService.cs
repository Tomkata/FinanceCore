namespace BankingSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Service for generating JWT tokens for authenticated users.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier</param>
        /// <param name="email">The user's email</param>
        /// <param name="roles">The user's roles</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(Guid userId, string email, IList<string> roles);
    }
}
