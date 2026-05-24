namespace SCH.Models.Students.ClientDtos
{
    using SCH.Models.Common.GridEntities;

    public class StudentGridRequest : GridRequest
    {
        public string? FirstNameValue1 { get; set; }
        public string? FirstNameOperator1 { get; set; }
        public string? FirstNameValue2 { get; set; }
        public string? FirstNameOperator2 { get; set; }
        public string? FirstNameFilterConcatOperator { get; set; }

        public string? LastNameValue1 { get; set; }
        public string? LastNameOperator1 { get; set; }
        public string? LastNameValue2 { get; set; }
        public string? LastNameOperator2 { get; set; }
        public string? LastNameFilterConcatOperator { get; set; }

        public string? EmailValue1 { get; set; }
        public string? EmailOperator1 { get; set; }
        public string? EmailValue2 { get; set; }
        public string? EmailOperator2 { get; set; }
        public string? EmailFilterConcatOperator { get; set; }

        public string? PhoneNumberValue1 { get; set; }
        public string? PhoneNumberOperator1 { get; set; }
        public string? PhoneNumberValue2 { get; set; }
        public string? PhoneNumberOperator2 { get; set; }
        public string? PhoneNumberFilterConcatOperator { get; set; }

        public string? SSNValue1 { get; set; }
        public string? SSNOperator1 { get; set; }
        public string? SSNValue2 { get; set; }
        public string? SSNOperator2 { get; set; }
        public string? SSNFilterConcatOperator { get; set; }

        /// <summary>ISO date string, e.g. "2024-01-15"</summary>
        public string? StartDate { get; set; }
        public string? StartDateOperator { get; set; }

        public bool? IsActive { get; set; }
    }
}
