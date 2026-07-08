namespace SCH.Models.Dashboard
{
    public class CourseStudentCountDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int StudentCount { get; set; }
    }
}
