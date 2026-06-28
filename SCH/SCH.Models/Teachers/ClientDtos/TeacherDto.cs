namespace SCH.Models.Teachers.ClientDtos
{
    using SCH.Models.Users.ClientDtos;

    public class TeacherDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? UserId { get; set; }

        public UserDomainDto? User { get; set; }

        /// <summary>
        /// Row version for optimistic concurrency control
        /// Must be sent back when updating to detect concurrent modifications
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
