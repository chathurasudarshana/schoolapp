namespace SCH.Core.Extensions
{
    using AutoMapper;
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Mappings;
    using System.Reflection;

    public static class MappingExtensions
    {
        public static void AddMappings(this IServiceCollection services)
        {
            Assembly assembly = Assembly.Load("SCH.Mappings");

            Type markerInterface = typeof(IProfile);

            Type[] profileTypes = assembly.GetTypes()
                .Where(t =>
                    markerInterface.IsAssignableFrom(t)
                    && !t.IsInterface
                    && !t.IsAbstract)
                .ToArray();

            services.AddAutoMapper(cfg =>
            {
                foreach (Type profileType in profileTypes)
                {
                    cfg.AddProfile(profileType);
                }
            });
        }
    }
}


