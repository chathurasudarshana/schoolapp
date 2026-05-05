namespace SCH.Models.Teachers.Entities
{
    using SCH.Models.Common.AuditableEntities;
    using SCH.Models.Common.ConcurrencyEntities;

    public class Teacher : IAuditableEntity, IConcurrencyEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        // Audit properties
        public int CreatedBy { get; set; }
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
