namespace SCH.Core.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SCH.Repositories.DbContexts;

    /// <summary>
    /// Extension methods for configuring database contexts
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Registers both IdentityContext (identity schema) and SCHContext (dbo schema)
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="connectionString">Connection string for the database</param>
        /// <param name="identityConnectionString">Optional separate connection string for Identity context</param>
        public static void AddDbContexts(
            this IServiceCollection services, 
            string? connectionString,
            string? identityConnectionString = null)
        {
            // Register IdentityContext (identity schema)
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    identityConnectionString ?? connectionString,
                    sqlOptions => sqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "identity")));

            // Register SCHContext (dbo schema)
            services.AddDbContext<SCHContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "dbo")));
        }
    }
}

