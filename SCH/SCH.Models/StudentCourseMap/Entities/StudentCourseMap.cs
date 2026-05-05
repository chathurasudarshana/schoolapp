namespace SCH.Models.StudentCourseMap.Entities
{
    using SCH.Models.Common.AuditableEntities;
    using SCH.Models.Common.ConcurrencyEntities;
    using SCH.Models.Courses.Entities;
    using SCH.Models.Students.Entities;

    public class StudentCourseMap : IAuditableEntity, IConcurrencyEntity
    {

        public int StudentId { get; set; }

        public Student? Student { get; set; }

        public int CourseId { get; set; }

        public Course? Course { get; set; }

        public DateTime EnrollmentDate { get; set; }

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
