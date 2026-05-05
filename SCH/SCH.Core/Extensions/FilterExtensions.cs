namespace SCH.Core.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Core.Filters;

    /// <summary>
    /// Extension methods for configuring MVC and filters
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Configures controllers with custom filters
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>MVC builder for additional configuration</returns>
        public static IMvcBuilder ConfigureControllersWithFilters(this IServiceCollection services)
        {
            return services.AddControllers(options =>
            {
                // Add model validation filter for consistent error responses
                options.Filters.Add<ModelValidationFilter>();
            });
        }
    }
}

