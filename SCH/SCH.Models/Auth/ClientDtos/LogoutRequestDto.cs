namespace SCH.Models.Auth.ClientDtos
{
    /// <summary>
    /// Request DTO for logout operation
    /// </summary>
    public class LogoutRequestDto
    {
        /// <summary>
        /// Refresh token to revoke (optional, can also come from header)
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}

