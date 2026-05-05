namespace SCH.Models.Auth.ClientDtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request model for user login
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Username or email
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}

