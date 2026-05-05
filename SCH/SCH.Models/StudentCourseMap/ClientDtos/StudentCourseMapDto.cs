using SCH.Models.Courses.Entities;
using SCH.Models.Students.Entities;

namespace SCH.Models.StudentCourseMap.ClientDtos
{
    public class StudentCourseMapDto
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public required DateTime EnrollmentDate { get; set; }

        public string? StudentFirstName { get; set; }

        public string? StudentLastName { get; set; }

        public string? CourseName { get; set; }
    }
}
