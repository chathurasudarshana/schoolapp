namespace SCH.Shared.HttpContext
{
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    /// <summary>
    /// Provides access to current user information from HttpContext
    /// Supports both dependency injection (via interface) and static access
    /// </summary>
    internal class UserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Instance Methods (for Dependency Injection)

        int? IUserInfo.GetCurrentUserId()
        {
            return GetCurrentUserId(_httpContextAccessor.HttpContext);
        }

        string? IUserInfo.GetCurrentUserName()
        {
            return GetCurrentUserName(_httpContextAccessor.HttpContext);
        }

        string? IUserInfo.GetCurrentUserEmail()
        {
            return GetCurrentUserEmail(_httpContextAccessor.HttpContext);
        }

        bool IUserInfo.IsAuthenticated()
        {
            return IsAuthenticated(_httpContextAccessor.HttpContext);
        }

        #endregion

        #region Static Methods (for direct access when HttpContext is available)

        /// <summary>
        /// Gets the current user's ID from JWT claims (ClaimTypes.NameIdentifier)
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        public static int? GetCurrentUserId(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return null;

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return null;
        }

        /// <summary>
        /// Gets the current user's username from JWT claims (ClaimTypes.Name)
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        public static string? GetCurrentUserName(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return null;

            return httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Gets the current user's email from JWT claims (ClaimTypes.Email)
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        public static string? GetCurrentUserEmail(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return null;

            return httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Checks if a user is currently authenticated in the given HttpContext
        /// </summary>
        public static bool IsAuthenticated(HttpContext? httpContext)
        {
            return httpContext?.User?.Identity?.IsAuthenticated == true;
        }

        #endregion
    }
}
