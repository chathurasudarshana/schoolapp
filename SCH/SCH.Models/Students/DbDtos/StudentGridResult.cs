namespace SCH.Models.Students.DbDtos
{
    using SCH.Models.Common.DbDtos;

    /// <summary>
    /// Maps the result set returned by dbo.GetStudentGrid.
    /// Not a public API DTO — used only by the repository layer.
    /// </summary>
    public class StudentGridResult : GridResult
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? SSN { get; set; }

        public string? Image { get; set; }

        public DateTime? StartDate { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public byte[] RowVersion { get; set; } = [];
    }
}
