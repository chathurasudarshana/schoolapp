namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Requirement: the editing user must be the owner of the student record.
    /// Checked by OwnStudentRecordHandler using the JWT own_student_id claim vs the route {id}.
    /// </summary>
    public class OwnStudentRecordRequirement : IAuthorizationRequirement { }

    public class OwnStudentRecordHandler : AuthorizationHandler<OwnStudentRecordRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OwnStudentRecordHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnStudentRecordRequirement requirement)
        {
            string? ownStudentIdClaim = context.User.FindFirst("own_student_id")?.Value;
            if (!string.IsNullOrEmpty(ownStudentIdClaim)) {
                RouteData? routeData = _httpContextAccessor.HttpContext?.GetRouteData();
                string? routeId = routeData?.Values["id"]?.ToString();

                if (!string.IsNullOrEmpty(routeId) && ownStudentIdClaim == routeId) {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
