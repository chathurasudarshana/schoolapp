namespace SCH.Repositories.UnitOfWork
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Common.AuditableEntities;
    using SCH.Repositories.DbContexts;
    using SCH.Shared.HttpContext;

    /// <summary>
    /// Unit of Work implementation for Identity context (identity schema)
    /// Handles audit tracking for identity entities using IIdentityAuditableEntity
    /// </summary>
    internal class IdentityUnitOfWork : BaseUnitOfWork<IdentityContext>, IIdentityUnitOfWork
    {
        public IdentityUnitOfWork(IdentityContext context, IUserInfo? userInfo = null)
            : base(context, userInfo)
        {
        }

        protected override void ApplyAuditTracking()
        {
            var currentUserId = UserInfo?.GetCurrentUserId();
            var timestamp = DateTime.UtcNow; // Single timestamp for all entities in this transaction
            var entries = Context.ChangeTracker.Entries<IIdentityAuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // Set CreatedDate if not already set
                    if (entry.Entity.CreatedDate == default)
                        entry.Entity.CreatedDate = timestamp;

                    // Set CreatedBy if user is authenticated (nullable for self-referencing)
                    if (currentUserId.HasValue)
                        entry.Entity.CreatedBy = currentUserId.Value;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Set ModifiedBy and ModifiedDate if user is authenticated
                    if (currentUserId.HasValue)
                    {
                        entry.Entity.ModifiedBy = currentUserId.Value;
                        entry.Entity.ModifiedDate = timestamp;
                    }
                }
            }
        }
    }
}

