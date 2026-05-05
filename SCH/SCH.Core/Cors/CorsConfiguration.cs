namespace SCH.Core.Cors
{
    using Microsoft.Extensions.DependencyInjection;

    public static class CorsConfiguration
    {
        public static void AddAllowedOrigins(
            this IServiceCollection services,
            string name,
            string? allowedOrigins)
        {
            string[] allowOrigins = new string[0];

            if (!string.IsNullOrWhiteSpace(allowedOrigins))
            {
                allowOrigins = allowedOrigins
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToArray();
            }

            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: name,
                    policy =>
                    {
                        policy
                            .WithOrigins(allowOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

        }
    }
}
