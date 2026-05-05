namespace SCH.Repositories.DbContexts
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Courses.Entities;
    using SCH.Models.Teachers.Entities;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Models.Students.Entities;
    using SCH.Models.Users.Entities;

    /// <summary>
    /// Database context for domain entities (dbo schema)
    /// </summary>
    public class SCHContext : DbContext
    {
        public SCHContext(DbContextOptions<SCHContext> options)
            : base(options)
        {
        }

        internal DbSet<User> Users { get; set; }

        internal DbSet<Student> Student { get; set; }

        internal DbSet<Course> Course { get; set; }

        internal DbSet<Teacher> Teacher { get; set; }

        internal DbSet<StudentCourseMap> StudentCourseMap { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default schema for all domain tables
            modelBuilder.HasDefaultSchema("dbo");

            // Configure User entity (domain user)
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "dbo");
                entity.HasKey(e => e.Id);

                // Id is manually set (not auto-generated) and references AspNetUsers.Id
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Audit columns - User has nullable CreatedBy (self-reference)
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student", "dbo");
                entity.Property(e => e.FirstName).HasColumnType("nvarchar(400)");
                entity.Property(e => e.LastName).HasColumnType("nvarchar(400)");
                entity.Property(e => e.Email).HasColumnType("nvarchar(400)");
                entity.Property(e => e.PhoneNumber).HasColumnType("nvarchar(20)");
                entity.Property(e => e.SSN).HasColumnType("nvarchar(20)");
                entity.Property(e => e.Image).HasColumnType("nvarchar(400)");
                entity.Property(e => e.StartDate).HasColumnType("date");

                // Audit foreign keys
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course", "dbo");
                entity.Property(e => e.Name).HasColumnType("nvarchar(400)");

                // Audit foreign keys
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            // Configure Teacher entity
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher", "dbo");
                entity.Property(e => e.Name).HasColumnType("nvarchar(400)");

                // Audit foreign keys
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            // Configure StudentCourseMap entity
            modelBuilder.Entity<StudentCourseMap>(entity =>
            {
                entity.ToTable("StudentCourseMap", "dbo");
                
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });

                entity.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentCourseMaps)
                    .HasForeignKey(sc => sc.StudentId);

                entity.HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentCourseMaps)
                    .HasForeignKey(sc => sc.CourseId);

                // Audit foreign keys
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                // Concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });
        }
    }
}
