namespace SCH.Models.Common.ConcurrencyEntities
{
    /// <summary>
    /// Interface for entities that support optimistic concurrency control
    /// Used by identity entities in identity schema
    /// Uses ConcurrencyStamp (string) instead of RowVersion (byte[])
    /// </summary>
    public interface IIdentityConcurrencyEntity
    {
        /// <summary>
        /// Concurrency stamp for optimistic concurrency control
        /// Must be updated on each modification
        /// </summary>
        string? ConcurrencyStamp { get; set; }
    }
}
