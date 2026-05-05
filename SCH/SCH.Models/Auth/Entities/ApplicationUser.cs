namespace SCH.Models.Auth.Entities
{
    using Microsoft.AspNetCore.Identity;
    using SCH.Models.Common.AuditableEntities;
    using SCH.Models.Common.ConcurrencyEntities;

    /// <summary>
    /// Represents an application user with custom properties
    /// ConcurrencyStamp is inherited from IdentityUser for optimistic concurrency control
    /// </summary>
    public class ApplicationUser : IdentityUser<int>, IIdentityAuditableEntity, IIdentityConcurrencyEntity
    {
        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the user was created (UTC)
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last login date and time
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Navigation property for refresh tokens
        /// </summary>
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Audit properties
        /// <summary>
        /// User ID who created this user (self-referencing FK to AspNetUsers.Id)
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// User ID who last modified this user (self-referencing FK to AspNetUsers.Id)
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Date and time when this user was last modified (UTC)
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
    }
}

