using Havit.Data.EntityFrameworkCore.Threading.Internal;
using Havit.Data.Patterns.DataSeeds;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;

/// <summary>
/// Zajišťuje spuštění seedování databáze.
/// Seedování je uzavřeno pod zámkem (bez ohledu na seedovaný profil).
/// </summary>
public class DbDataSeedRunner : DataSeedRunner
{
	private const string DataSeedLockValue = "DbDataSeeds";
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbDataSeedRunner(IEnumerable<IDataSeed> dataSeeds,
		IDataSeedRunDecision dataSeedRunDecision,
		IDataSeedPersisterFactory dataSeedPersisterFactory,
		IDbContext dbContext)
		: base(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactory)
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
				// podpora pro Connection Resiliency
				// seedování používá "Option 2 - Rebuild application state" popsanou v dokumentaci
				// viz: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
				var strategy = dbContext.Database.CreateExecutionStrategy();
				strategy.Execute(() =>
				{
					using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
					{
						base.SeedData(dataSeedProfileType, forceRun);
						transaction.Commit();
					}
				});
			});
		}
		else
		{
			base.SeedData(dataSeedProfileType, forceRun);
		}
	}
}
