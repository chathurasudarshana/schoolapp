namespace SCH.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Services;
    using System.Reflection;

    /// <summary>
    /// Extension methods for configuring application services
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Automatically registers all service implementations from SCH.Services assembly
        /// </summary>
        /// <param name="services">Service collection</param>
        public static void AddServices(this IServiceCollection services)
        {
            Assembly assembly = Assembly.Load("SCH.Services");

            Type superInterfaceType = typeof(IService);

            IEnumerable<Type> types = assembly.GetTypes()
                .Where(t => 
                    superInterfaceType.IsAssignableFrom(t) 
                    && !t.IsInterface 
                    && !t.IsAbstract);

            foreach (Type type in types)
            {
                Type parentInteface = type.GetInterfaces()
                    .Single(i =>
                        i.GetInterfaces()
                        .Any(ip => ip == superInterfaceType));

                services.AddScoped(parentInteface, type);
            }
        }
    }
}

