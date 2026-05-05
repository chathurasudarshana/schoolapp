namespace SCH.Models.Auth.ClientDtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request model for changing password
    /// </summary>
    public class ChangePasswordRequestDto
    {
        /// <summary>
        /// Current password
        /// </summary>
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirm new password
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

