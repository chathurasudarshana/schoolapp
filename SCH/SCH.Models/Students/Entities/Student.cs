namespace SCH.Models.Students.Entities
{
    using SCH.Models.Common.AuditableEntities;
    using SCH.Models.Common.ConcurrencyEntities;
    using SCH.Models.StudentCourseMap.Entities;

    public class Student : IAuditableEntity, IConcurrencyEntity
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? SSN { get; set; }

        public string? Image { get; set; }

        public DateTime? StartDate { get; set; }

        public bool IsActive { get; set; }

        public required ICollection<StudentCourseMap> StudentCourseMaps { get; set; }

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
