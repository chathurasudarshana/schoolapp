namespace SCH.Repositories.Common
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Common.ConcurrencyEntities;

    /// <summary>
    /// Base repository with common data access patterns
    /// Provides generic concurrency control support
    /// </summary>
    internal abstract class BaseRepository<TEntity, TContext> 
        where TEntity : class, IConcurrencyEntity
        where TContext : DbContext
    {
        protected readonly TContext Context;

        protected BaseRepository(TContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Updates entity with optimistic concurrency check
        /// Uses the provided originalRowVersion for WHERE clause comparison
        /// </summary>
        /// <param name="entity">Entity with updated properties and RowVersion from frontend</param>
        protected void UpdateWithConcurrency(TEntity entity)
        {
            // Update - EF Core will use entity.RowVersion in WHERE clause
            Context.Update(entity);
        }
    }
}
