namespace SCH.Models.Auth.ClientDtos
{
    /// <summary>
    /// Response model for successful login
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT access token (short-lived)
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token (long-lived)
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token expiration time in seconds
        /// </summary>
        public int RefreshTokenExpiresIn { get; set; }

        /// <summary>
        /// Token type (usually "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// User information
        /// </summary>
        public UserDto User { get; set; } = null!;
    }
}

