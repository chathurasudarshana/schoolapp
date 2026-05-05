namespace SCH.Models.Users.Entities
{
    using SCH.Models.Common.AuditableEntities;
    using SCH.Models.Common.ConcurrencyEntities;

    /// <summary>
    /// Domain user entity - contains domain-specific user information
    /// Id is the same as ApplicationUser.Id from Identity schema
    /// </summary>
    public class User : IAuditableEntity, IConcurrencyEntity
    {
        /// <summary>
        /// Unique identifier for the domain user (same as AspNetUsers.Id)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Full name computed property
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Audit properties
        // Note: User.CreatedBy must be nullable (not int) for self-referencing FK
        // We'll need to handle this specially in SaveChangesAsync
        int IAuditableEntity.CreatedBy
        {
            get => CreatedBy ?? 0;
            set => CreatedBy = value;
        }

        /// <summary>
        /// User ID who created this user (self-referencing FK, nullable)
        /// </summary>
        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Concurrency control
        /// <summary>
        /// Row version for optimistic concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

