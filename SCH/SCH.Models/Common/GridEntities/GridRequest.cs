namespace SCH.Models.Common.GridEntities
{
    public class GridRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public string? SortBy { get; set; }

        /// <summary>"asc" or "desc"</summary>
        public string? SortByOperator { get; set; }
    }
}
