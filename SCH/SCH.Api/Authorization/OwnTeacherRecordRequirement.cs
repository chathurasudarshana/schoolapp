namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Requirement: the editing user must be the owner of the teacher record.
    /// Checked by OwnTeacherRecordHandler using the JWT own_teacher_id claim vs the route {id}.
    /// </summary>
    public class OwnTeacherRecordRequirement : IAuthorizationRequirement { }

    public class OwnTeacherRecordHandler : AuthorizationHandler<OwnTeacherRecordRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OwnTeacherRecordHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnTeacherRecordRequirement requirement)
        {
            string? ownTeacherIdClaim = context.User.FindFirst("own_teacher_id")?.Value;
            if (!string.IsNullOrEmpty(ownTeacherIdClaim)) {
                RouteData? routeData = _httpContextAccessor.HttpContext?.GetRouteData();
                string? routeId = routeData?.Values["id"]?.ToString();

                if (!string.IsNullOrEmpty(routeId) && ownTeacherIdClaim == routeId) {
                    context.Succeed(requirement);
                }

            }

            return Task.CompletedTask;
        }
    }
}
