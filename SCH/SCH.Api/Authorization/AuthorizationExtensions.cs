namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using SCH.Models.Auth.Constants;

    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddSchoolAppPolicies(this IServiceCollection services)
        {
            // Register own-record handlers
            services.AddSingleton<IAuthorizationHandler, StudentRecordEditAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TeacherRecordEditAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                // ---- Student policies ----

                // Any role with students:read, or Admin, or Teacher role, or Student role
                options.AddPolicy(Policy.ViewStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.IsInRole(Role.Teacher) ||
                        ctx.User.IsInRole(Role.Student) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Student.Read)));

                // Admin or Teacher (has students:add claim via role)
                options.AddPolicy(Policy.AddStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Student.Add)));

                // Admin, Teacher (students:write) can edit any record; students with write-own can edit their own
                options.AddPolicy(Policy.EditStudents, policy =>
                    policy.AddRequirements(new StudentRecordEditAuthorizationRequirement()));

                // Admin only
                options.AddPolicy(Policy.DeleteStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Student.Remove)));

                // ---- Teacher policies ----

                options.AddPolicy(Policy.ViewTeachers, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.IsInRole(Role.Teacher) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Teacher.Read)));

                // Admin, or Teacher editing their own record (teachers:write-own)
                options.AddPolicy(Policy.EditTeachers, policy =>
                    policy.AddRequirements(new TeacherRecordEditAuthorizationRequirement()));

                options.AddPolicy(Policy.DeleteTeachers, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Teacher.Remove)));

                // ---- Course policies ----

                options.AddPolicy(Policy.ViewCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.IsInRole(Role.Teacher) ||
                        ctx.User.IsInRole(Role.Student) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Course.Read)));

                options.AddPolicy(Policy.AddCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Course.Add)));

                options.AddPolicy(Policy.EditCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Course.Write)));

                options.AddPolicy(Policy.DeleteCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole(Role.Admin) ||
                        ctx.User.HasClaim(Permission.ClaimType, Permission.Course.Remove)));

            });

            return services;
        }
    }
}
