namespace SCH.Repositories.DbContexts
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Design-time factory for SCHContext migrations
    /// </summary>
    public class SCHContextFactory : IDesignTimeDbContextFactory<SCHContext>
    {
        public SCHContext CreateDbContext(string[] args)
        {
            // Build configuration
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../SCH.Api");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get connection string
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<SCHContext>();
            
            // Configure with dbo schema migration history
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "dbo"));

            return new SCHContext(optionsBuilder.Options);
        }
    }
}