namespace SCH.Models.Common.AuditableEntities
{
    /// <summary>
    /// Interface for entities that support audit tracking
    /// Used by domain entities in dbo schema
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// User ID who created this entity (FK to dbo.User.Id)
        /// </summary>
        int CreatedBy { get; set; }

        /// <summary>
        /// Date and time when this entity was created (UTC)
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// User ID who last modified this entity (FK to dbo.User.Id)
        /// </summary>
        int? ModifiedBy { get; set; }

        /// <summary>
        /// Date and time when this entity was last modified (UTC)
        /// </summary>
        DateTime? ModifiedDate { get; set; }
    }
}
