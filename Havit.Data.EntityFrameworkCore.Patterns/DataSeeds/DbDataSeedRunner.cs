using Havit.Data.EntityFrameworkCore.Threading.Internal;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Transactions.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds
{
    /// <summary>
    /// Zajišťuje spuštění seedování databáze.
    /// Seedování je uzavřeno pod zámkem (bez ohledu na seedovaný profil).
    /// </summary>
    public class DbDataSeedRunner : DataSeedRunner
    {
        private const string DataSeedLockValue = "DbDataSeeds";
        private readonly IDbContextTransient dbContext;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public DbDataSeedRunner(IEnumerable<IDataSeed> dataSeeds,
            IDataSeedRunDecision dataSeedRunDecision,
            IDataSeedPersisterFactory dataSeedPersisterFactory,
            ITransactionWrapper transactionWrapper,
            IDbContextTransient dbContext) 
            : base(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactory, transactionWrapper)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public override void SeedData(Type dataSeedProfileType, bool forceRun = false)
        {
            if (dbContext.Database.IsSqlServer())
            {
                new DbLockedCriticalSection((SqlConnection)dbContext.Database.GetDbConnection()).ExecuteAction(DataSeedLockValue, () =>
                {
                    base.SeedData(dataSeedProfileType, forceRun);
                });
            }
            else
            {
                base.SeedData(dataSeedProfileType, forceRun);
            }
        }
    }
}
