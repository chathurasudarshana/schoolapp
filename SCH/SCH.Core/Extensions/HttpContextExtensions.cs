namespace SCH.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Shared.HttpContext;
    using System.Reflection;

    /// <summary>
    /// Extension methods for configuring HttpContext-related services
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Automatically registers all HttpContext service implementations from SCH.Shared assembly
        /// Also registers IHttpContextAccessor which is required for these services
        /// </summary>
        /// <param name="services">Service collection</param>
        public static void AddHttpContextServices(this IServiceCollection services)
        {
            // Register IHttpContextAccessor (required for accessing HttpContext)
            services.AddHttpContextAccessor();

            Assembly assembly = Assembly.Load("SCH.Shared");

            Type superInterfaceType = typeof(IHttpContextService);

            IEnumerable<Type> types = assembly.GetTypes()
                .Where(t => 
                    superInterfaceType.IsAssignableFrom(t) 
                    && !t.IsInterface 
                    && !t.IsAbstract);

            foreach (Type type in types)
            {
                Type parentInterface = type.GetInterfaces()
                    .Single(i => 
                        i.GetInterfaces()
                        .Any(ip => ip == superInterfaceType));

                services.AddScoped(parentInterface, type);
            }
        }
    }
}

