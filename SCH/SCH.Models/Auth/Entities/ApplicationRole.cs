namespace SCH.Models.Auth.Entities
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Represents an application role
    /// </summary>
    public class ApplicationRole : IdentityRole<int>
    {
        /// <summary>
        /// Role description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indicates if the role is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the role was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

