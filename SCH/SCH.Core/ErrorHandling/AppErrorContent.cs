namespace SCH.Core.ErrorHandling
{
    using System.Collections;

    public class AppErrorContent
    {
        public required string Message { get; set; }

        public string? Trace { get; set; }

        public IDictionary? Data { get; set; }
    }
}
