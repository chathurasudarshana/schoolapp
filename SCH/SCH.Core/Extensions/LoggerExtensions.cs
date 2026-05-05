namespace SCH.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Shared.Logger;

    /// <summary>
    /// Extension methods for configuring logging services
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Registers custom application logger
        /// </summary>
        /// <param name="services">Service collection</param>
        public static void AddLogger(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        }
    }
}

