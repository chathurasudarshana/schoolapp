namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using SCH.Models.Auth.Constants;

    public class TeacherRecordEditAuthorizationHandler : AuthorizationHandler<TeacherRecordEditAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeacherRecordEditAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TeacherRecordEditAuthorizationRequirement requirement)
        {

            // Admin or users with full teachers:write permission can edit any record
            if (context.User.IsInRole(Role.Admin) ||
                context.User.HasClaim(Permission.ClaimType, Permission.Teacher.Write))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Users with teachers:write-own can only edit their own record
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
