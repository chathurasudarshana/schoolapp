namespace SCH.Shared.HttpContext
{
    /// <summary>
    /// Interface for accessing current user information from HttpContext
    /// </summary>
    public interface IUserInfo : IHttpContextService
    {
        /// <summary>
        /// Gets the current authenticated user's ID from JWT claims
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        int? GetCurrentUserId();

        /// <summary>
        /// Gets the current authenticated user's username from JWT claims
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        string? GetCurrentUserName();

        /// <summary>
        /// Gets the current authenticated user's email from JWT claims
        /// Returns null if no user is authenticated or if the claim is invalid
        /// </summary>
        string? GetCurrentUserEmail();

        /// <summary>
        /// Checks if a user is currently authenticated
        /// </summary>
        bool IsAuthenticated();
    }
}
