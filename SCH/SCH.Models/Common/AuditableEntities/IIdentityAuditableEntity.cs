namespace SCH.Models.Common.AuditableEntities
{
    /// <summary>
    /// Interface for entities that support audit tracking
    /// Used by identity entities in identity schema
    /// CreatedBy is nullable to support self-referencing foreign keys
    /// </summary>
    public interface IIdentityAuditableEntity
    {
        /// <summary>
        /// User ID who created this entity (FK to identity.AspNetUsers.Id)
        /// Nullable for self-referencing entities
        /// </summary>
        int? CreatedBy { get; set; }

        /// <summary>
        /// Date and time when this entity was created (UTC)
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// User ID who last modified this entity (FK to identity.AspNetUsers.Id)
        /// </summary>
        int? ModifiedBy { get; set; }

        /// <summary>
        /// Date and time when this entity was last modified (UTC)
        /// </summary>
        DateTime? ModifiedDate { get; set; }
    }
}
