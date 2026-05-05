namespace SCH.Shared.Exceptions
{
    /// <summary>
    /// Global error codes for the application
    /// These codes should match the TypeScript SCHErrorNumber enum in the client
    /// </summary>
    public enum SCHErrorNumber
    {
        /// <summary>
        /// General/unknown error
        /// </summary>
        None = 0,

        /// <summary>
        /// Concurrency conflict - record was modified by another user
        /// </summary>
        ConcurrencyConflict = 1
    }
}
