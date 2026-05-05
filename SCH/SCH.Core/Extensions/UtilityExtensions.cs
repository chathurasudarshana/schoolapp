namespace SCH.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Shared.Utility;
    using System.Reflection;

    /// <summary>
    /// Extension methods for configuring utility services
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// Automatically registers all utility implementations from SCH.Shared assembly
        /// </summary>
        /// <param name="services">Service collection</param>
        public static void AddUtilities(this IServiceCollection services)
        {
            Assembly assembly = Assembly.Load("SCH.Shared");

            Type superInterfaceType = typeof(IUtility);

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

