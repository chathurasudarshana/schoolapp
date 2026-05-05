namespace SCH.Core.Extensions
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using SCH.Models.Auth.Entities;
    using SCH.Repositories.DbContexts;
    using System.Text;

    /// <summary>
    /// Extension methods for configuring authentication and authorization
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Configures ASP.NET Identity with JWT Bearer authentication
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        public static void AddAuthenticationWithJwt(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure ASP.NET Identity (uses IdentityContext with "identity" schema)
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = configuration.GetValue<bool>("IdentitySettings:Password:RequireDigit");
                options.Password.RequireLowercase = configuration.GetValue<bool>("IdentitySettings:Password:RequireLowercase");
                options.Password.RequireUppercase = configuration.GetValue<bool>("IdentitySettings:Password:RequireUppercase");
                options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("IdentitySettings:Password:RequireNonAlphanumeric");
                options.Password.RequiredLength = configuration.GetValue<int>("IdentitySettings:Password:RequiredLength");
                options.Password.RequiredUniqueChars = configuration.GetValue<int>("IdentitySettings:Password:RequiredUniqueChars");

                // Lockout settings
                options.Lockout.AllowedForNewUsers = configuration.GetValue<bool>("IdentitySettings:Lockout:AllowedForNewUsers");
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
                    configuration.GetValue<int>("IdentitySettings:Lockout:DefaultLockoutTimeSpanMinutes"));
                options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("IdentitySettings:Lockout:MaxFailedAccessAttempts");

                // User settings
                options.User.RequireUniqueEmail = configuration.GetValue<bool>("IdentitySettings:User:RequireUniqueEmail");

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>("IdentitySettings:SignIn:RequireConfirmedEmail");
                options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>("IdentitySettings:SignIn:RequireConfirmedPhoneNumber");
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Remove default 5-minute tolerance
                };
            });

            services.AddAuthorization();
        }
    }
}

