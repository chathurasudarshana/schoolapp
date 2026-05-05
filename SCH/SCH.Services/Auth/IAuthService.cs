namespace SCH.Services.Auth
{
    using SCH.Models.Auth.ClientDtos;
    using SCH.Models.Auth.Enums;

    /// <summary>
    /// Service for authentication and authorization operations
    /// </summary>
    public interface IAuthService : IService
    {
        /// <summary>
        /// Authenticates a user and generates tokens
        /// </summary>
        /// <param name="request">Login request with credentials</param>
        /// <param name="ipAddress">Client IP address</param>
        /// <param name="userAgent">Client user agent</param>
        /// <returns>Login response with tokens and user info</returns>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, string? userAgent);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">Registration request with user details</param>
        /// <returns>Login response with tokens and user info</returns>
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);

        /// <summary>
        /// Refreshes an access token using a refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <param name="ipAddress">Client IP address</param>
        /// <param name="userAgent">Client user agent</param>
        /// <returns>Login response with new tokens</returns>
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, string? userAgent);

        /// <summary>
        /// Logs out a user and revokes their refresh tokens based on scope
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="currentRefreshToken">Current refresh token (optional)</param>
        /// <param name="scope">Logout scope - determines which tokens to revoke</param>
        /// <returns>Task</returns>
        Task LogoutAsync(int userId, string? currentRefreshToken = null, LogoutScope scope = LogoutScope.CurrentBrowser);

        /// <summary>
        /// Revokes a specific refresh token
        /// </summary>
        /// <param name="tokenId">Token ID</param>
        /// <param name="userId">User ID (for authorization)</param>
        /// <returns>Task</returns>
        Task RevokeTokenAsync(int tokenId, int userId);

        /// <summary>
        /// Gets all active sessions for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of active sessions</returns>
        Task<List<SessionDto>> GetActiveSessionsAsync(int userId);

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Change password request</param>
        /// <returns>Task</returns>
        Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);

        /// <summary>
        /// Checks if a username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if available, false if taken</returns>
        Task<bool> IsUsernameAvailableAsync(string username);

        /// <summary>
        /// Checks if an email is available
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>True if available, false if taken</returns>
        Task<bool> IsEmailAvailableAsync(string email);
    }
}

