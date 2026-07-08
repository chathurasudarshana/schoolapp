namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using SCH.Models.Auth.Constants;
    using SCH.Shared.HttpContext;

    public class StudentRecordEditAuthorizationHandler : AuthorizationHandler<StudentRecordEditAuthorizationRequirement>
    {
        private readonly IRouteInfo _routeInfo;

        public StudentRecordEditAuthorizationHandler(IRouteInfo routeInfo)
        {
            _routeInfo = routeInfo;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            StudentRecordEditAuthorizationRequirement requirement)
        {
            // Admin or users with full students:write permission can edit any record
            if (context.User.IsInRole(Role.Admin) ||
                context.User.HasClaim(Permission.ClaimType, Permission.Student.Write))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Users with students:write-own can only edit their own record
            string? ownStudentIdClaim = context.User.FindFirst("own_student_id")?.Value;
            if (!string.IsNullOrEmpty(ownStudentIdClaim))
            {
                string? routeId = _routeInfo.GetRouteDataValue("id");

                if (!string.IsNullOrEmpty(routeId) && ownStudentIdClaim == routeId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
