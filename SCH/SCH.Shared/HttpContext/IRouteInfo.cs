namespace SCH.Shared.HttpContext
{
    /// <summary>
    /// Interface for accessing route data from the current HttpContext
    /// </summary>
    public interface IRouteInfo : IHttpContextService
    {
        /// <summary>
        /// Gets a route data value by key from the current request
        /// Returns null if no route data is found or the key does not exist
        /// </summary>
        string? GetRouteDataValue(string key);
    }
}
