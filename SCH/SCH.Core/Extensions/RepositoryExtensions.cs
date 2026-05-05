namespace SCH.Core.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Repositories;
    using System.Reflection;

    /// <summary>
    /// Extension methods for configuring repository services
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Automatically registers all repository implementations from SCH.Repositories assembly
        /// </summary>
        /// <param name="services">Service collection</param>
        public static void AddRepositories(this IServiceCollection services)
        {
            Assembly assembly = Assembly.Load("SCH.Repositories");

            Type superInterfaceType = typeof(IRepository);

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

