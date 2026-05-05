using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCH.Repositories
{
    /// <summary>
    /// Base Unit of Work interface for managing database transactions
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes made in this unit of work to the database
        /// Uses implicit transaction (all changes are atomic)
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Begins an explicit database transaction
        /// Use this when you need multiple SaveChangesAsync() calls to be atomic
        /// Must call CommitTransactionAsync() or RollbackTransactionAsync() when done
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current transaction
        /// All changes made within the transaction are persisted to the database
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction
        /// All changes made within the transaction are discarded
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
