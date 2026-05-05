namespace SCH.Models.Students.ClientDtos
{
    using SCH.Models.StudentCourseMap.ClientDtos;

    public class StudentDto
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

        public List<StudentCourseMapDto> Courses { get; set; } 
            = new List<StudentCourseMapDto>();

        /// <summary>
        /// Row version for optimistic concurrency control
        /// Must be sent back when updating to detect concurrent modifications
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
