using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
    /// <inheritdoc />
    public class DbDataSeedTransactionContext : IDbDataSeedTransactionContext
    {
        /// <inheritdoc />
        public IDbContextTransaction CurrentTransaction { get; set; }

        /// <inheritdoc />
        public void ApplyCurrentTransactionTo(IDbContext targetDbContext)
        {            
            Contract.Requires(CurrentTransaction != null);
            Contract.Requires(targetDbContext.Database.IsSqlServer());

            var transaction = CurrentTransaction.GetDbTransaction();
            targetDbContext.Database.SetDbConnection(transaction.Connection);
            targetDbContext.Database.UseTransaction(transaction);
        }
    }
}