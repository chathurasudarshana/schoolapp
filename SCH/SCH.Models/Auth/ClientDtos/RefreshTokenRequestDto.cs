namespace SCH.Models.Auth.ClientDtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request model for refreshing access token
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// The expired or about-to-expire access token
        /// </summary>
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// The refresh token
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

