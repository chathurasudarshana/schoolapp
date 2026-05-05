namespace SCH.Models.Courses.ClientDtos
{
    public class CourseDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        /// <summary>
        /// Row version for optimistic concurrency control
        /// Must be sent back when updating to detect concurrent modifications
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
