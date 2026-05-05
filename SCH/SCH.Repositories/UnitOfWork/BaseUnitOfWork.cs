namespace SCH.Repositories.UnitOfWork
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using SCH.Shared.HttpContext;

    /// <summary>
    /// Abstract base class for Unit of Work implementations
    /// Provides common transaction management and delegates audit tracking to derived classes
    /// </summary>
    /// <typeparam name="TContext">The DbContext type</typeparam>
    internal abstract class BaseUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        protected readonly TContext Context;
        protected readonly IUserInfo? UserInfo;
        private IDbContextTransaction? _currentTransaction;

        protected BaseUnitOfWork(TContext context, IUserInfo? userInfo = null)
        {
            Context = context;
            UserInfo = userInfo;
        }

        public async Task SaveChangesAsync()
        {
            // Apply audit tracking (implemented by derived classes)
            ApplyAuditTracking();

            // Save changes to the database
            await Context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        /// <summary>
        /// Apply audit tracking to entities before saving
        /// Must be implemented by derived classes to handle specific audit entity types
        /// </summary>
        protected abstract void ApplyAuditTracking();
    }
}
