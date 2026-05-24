namespace SCH.Models.Students.ClientDtos
{
    using SCH.Models.Common.GridEntities;

    public class StudentGridRequest : GridRequest
    {
        public string? FirstName { get; set; }
        public string? FirstNameOperator { get; set; }

        public string? LastName { get; set; }
        public string? LastNameOperator { get; set; }

        public string? Email { get; set; }
        public string? EmailOperator { get; set; }

        public string? PhoneNumber { get; set; }
        public string? PhoneNumberOperator { get; set; }

        public string? SSN { get; set; }
        public string? SSNOperator { get; set; }

        /// <summary>ISO date string, e.g. "2024-01-15"</summary>
        public string? StartDate { get; set; }
        public string? StartDateOperator { get; set; }

        public bool? IsActive { get; set; }
    }
}
