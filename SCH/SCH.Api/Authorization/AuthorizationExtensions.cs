namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddSchoolAppPolicies(this IServiceCollection services)
        {
            // Register own-record handlers
            services.AddSingleton<IAuthorizationHandler, OwnStudentRecordHandler>();
            services.AddSingleton<IAuthorizationHandler, OwnTeacherRecordHandler>();

            services.AddAuthorization(options =>
            {
                // ---- Student policies ----

                // Any role with students:read, or Admin, or Teacher role, or Student role
                options.AddPolicy(PolicyNames.ViewStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.IsInRole("Teacher") ||
                        ctx.User.IsInRole("Student") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Students.Read)));

                // Admin or Teacher (has students:add claim via role)
                options.AddPolicy(PolicyNames.AddStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Students.Add)));

                // Admin, Teacher (students:write), or any user editing their own record (students:write-own)
                options.AddPolicy(PolicyNames.EditStudents, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Students.Write) ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Students.WriteOwn)));

                // Edit own student record only (Student role)
                options.AddPolicy(PolicyNames.EditOwnStudent, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Students.Write))
                    .AddRequirements(new OwnStudentRecordRequirement()));

                // Admin only
                options.AddPolicy(PolicyNames.DeleteStudents, policy =>
                    policy.RequireRole("Admin"));

                // ---- Teacher policies ----

                options.AddPolicy(PolicyNames.ViewTeachers, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.IsInRole("Teacher") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Teachers.Read)));

                // Admin, or Teacher editing their own record (teachers:write-own)
                options.AddPolicy(PolicyNames.EditTeachers, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Teachers.Write) ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Teachers.WriteOwn)));

                options.AddPolicy(PolicyNames.EditOwnTeacher, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Teachers.Write))
                    .AddRequirements(new OwnTeacherRecordRequirement()));

                options.AddPolicy(PolicyNames.DeleteTeachers, policy =>
                    policy.RequireRole("Admin"));

                // ---- Course policies ----

                options.AddPolicy(PolicyNames.ViewCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.IsInRole("Teacher") ||
                        ctx.User.IsInRole("Student") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Courses.Read)));

                options.AddPolicy(PolicyNames.AddCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Courses.Add)));

                options.AddPolicy(PolicyNames.EditCourses, policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim(Permissions.ClaimType, Permissions.Courses.Write)));

                options.AddPolicy(PolicyNames.DeleteCourses, policy =>
                    policy.RequireRole("Admin"));
            });

            return services;
        }
    }
}
