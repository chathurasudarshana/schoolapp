namespace SCH.Shared.HttpContext
{
    using Microsoft.AspNetCore.Http;

    internal class RouteInfo : IRouteInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RouteInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        string? IRouteInfo.GetRouteDataValue(string key)
        {
            return GetRouteDataValue(_httpContextAccessor.HttpContext, key);
        }

        public static string? GetRouteDataValue(HttpContext? httpContext, string key)
        {
            return httpContext?.Request.RouteValues[key]?.ToString();
        }
    }
}
