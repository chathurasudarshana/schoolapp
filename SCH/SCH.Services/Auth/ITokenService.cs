namespace SCH.Services.Auth
{
    using System.Security.Claims;

    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface ITokenService : IService
    {
        /// <summary>
        /// Generates a JWT access token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="username">Username</param>
        /// <param name="email">User email</param>
        /// <param name="roles">User roles</param>
        /// <returns>JWT token string and token ID (jti)</returns>
        Task<(string Token, string TokenId)> GenerateAccessTokenAsync(
            int userId, 
            string username, 
            string email, 
            IList<string> roles);

        /// <summary>
        /// Generates a random refresh token
        /// </summary>
        /// <returns>Refresh token string</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Gets the principal from an expired token (for refresh token validation)
        /// </summary>
        /// <param name="token">The expired access token</param>
        /// <returns>ClaimsPrincipal or null if invalid</returns>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        /// <summary>
        /// Validates a token without checking expiration
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>True if valid structure, false otherwise</returns>
        bool ValidateTokenStructure(string token);
    }
}

