namespace SCH.Models.Auth.ClientDtos
{
    /// <summary>
    /// User data transfer object
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User roles
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Indicates if the user is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Date when the user was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Concurrency stamp for optimistic concurrency control
        /// Must be sent back when updating to detect concurrent modifications
        /// </summary>
        public string? ConcurrencyStamp { get; set; }
    }
}

