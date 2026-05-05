namespace SCH.Repositories.DbContexts
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Auth.Entities;

    /// <summary>
    /// Database context for authentication and authorization (Identity schema)
    /// </summary>
    public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        internal DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set default schema for all Identity tables
            builder.HasDefaultSchema("identity");

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers", "identity");
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);

                // Audit foreign keys - self-referencing, nullable
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control - ConcurrencyStamp inherited from IdentityUser
                entity.Property(e => e.ConcurrencyStamp)
                    .IsConcurrencyToken();
            });

            // Configure ApplicationRole
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("AspNetRoles", "identity");
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            // Explicitly configure other Identity tables with schema
            builder.Entity<IdentityUserRole<int>>()
                .ToTable("AspNetUserRoles", "identity");

            builder.Entity<IdentityUserClaim<int>>()
                .ToTable("AspNetUserClaims", "identity");

            builder.Entity<IdentityUserLogin<int>>()
                .ToTable("AspNetUserLogins", "identity");

            builder.Entity<IdentityUserToken<int>>()
                .ToTable("AspNetUserTokens", "identity");

            builder.Entity<IdentityRoleClaim<int>>()
                .ToTable("AspNetRoleClaims", "identity");

            // Configure RefreshToken entity
            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens", "identity");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.JwtId)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(45); // IPv6 max length

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);

                entity.Property(e => e.DeviceName)
                    .HasMaxLength(100);

                // Create indexes for performance
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpiryDate);
                entity.HasIndex(e => e.IsRevoked);
                entity.HasIndex(e => e.FamilyId)
                    .HasFilter("[IsRevoked] = 0"); // Filtered index for better performance
                entity.HasIndex(e => e.ParentTokenId);

                // Configure relationship with ApplicationUser
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure self-referencing relationship (Parent token)
                // Note: No cascade delete to prevent issues with token chains
                entity.HasOne<RefreshToken>()
                    .WithMany()
                    .HasForeignKey(e => e.ParentTokenId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });
        }
    }
}

