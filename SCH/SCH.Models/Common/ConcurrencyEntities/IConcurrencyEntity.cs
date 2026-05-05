namespace SCH.Models.Common.ConcurrencyEntities
{
    /// <summary>
    /// Interface for entities that support optimistic concurrency control
    /// Used by domain entities in dbo schema
    /// </summary>
    public interface IConcurrencyEntity
    {
        /// <summary>
        /// Row version for optimistic concurrency control
        /// Automatically updated by SQL Server on each update
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
