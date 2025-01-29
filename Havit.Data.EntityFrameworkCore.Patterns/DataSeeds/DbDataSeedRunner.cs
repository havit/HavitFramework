using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Threading;
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
	private readonly IEntityCacheManager _entityCacheManager;
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbDataSeedRunner(IEnumerable<IDataSeed> dataSeeds,
		IDataSeedRunDecision dataSeedRunDecision,
		IDataSeedPersisterFactory dataSeedPersisterFactory,
		IEntityCacheManager entityCacheManager,
		IDbContext dbContext)
		: base(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactory)
	{
		_entityCacheManager = entityCacheManager;
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public override void SeedData(Type dataSeedProfileType, bool forceRun = false)
	{
		SeedAsyncFromSyncSeedDataException seedAsyncFromSyncSeedDataException = null;

		try
		{
			if (_dbContext.Database.IsSqlServer())
			{
				new DbLockedCriticalSection((SqlConnection)_dbContext.Database.GetDbConnection()).ExecuteAction(DataSeedLockValue, () =>
				{
					// podpora pro Connection Resiliency
					// seedování používá "Option 2 - Rebuild application state" popsanou v dokumentaci
					// viz: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
					var strategy = _dbContext.Database.CreateExecutionStrategy();
					strategy.ExecuteInTransaction(() =>
					{
						try
						{
							base.SeedData(dataSeedProfileType, forceRun);
						}
						catch (SeedAsyncFromSyncSeedDataException exception)
						{
							// Při chybném volání async metody z sync seedu nebo zapomenutí awaitu zůstává typicky otevřené spojení (a reader).
							// Různá následující volání (commit, rollback, Dispose!!!) pak selhávají. Ovšem typicky ve finally bloku,
							// takže je tato výjimka je zahozena. (C# 4 Language Specification § 8.9.5: If the finally block throws another exception, processing of the current exception is terminated.)
							// Pokud již nedošlo k zamaskování, pokusíme se výjimkou, která nás zajíma zde zapamatovat,
							// abychom ji níže mohli vyhodit v AggregateException.
							// Cílem je, dát programátorovi vědět zdrojovou chybu, nikoliv následné chyby ve stylu "Na SQL spojení není povolen MARS." atp.
							seedAsyncFromSyncSeedDataException = exception;
							throw;
						}
					},
					null);
				});
			}
			else
			{
				base.SeedData(dataSeedProfileType, forceRun);
			}
		}
		catch (Exception exception) when ((seedAsyncFromSyncSeedDataException != null) && (exception != seedAsyncFromSyncSeedDataException /* to se snad nemůže stát */))
		{
			_entityCacheManager.InvalidateAll(); // viz komentář níže u "catch"

			// viz komentář výše u přiřazení seedAsyncFromSyncSeedDataException
			throw new AggregateException(seedAsyncFromSyncSeedDataException, exception);
		}
		catch
		{
			// Pokud neprošlo seedování celé, mohlo se stát, že proběhla jeho část v transakci a došlo k uložení části dat (později rollback-ovaných)
			// do cache. Proto preventivně z cache odstraníme veškerá data (vzhledem ke kontextu použití této služby si to zřejmě můžeme dovolit).
			_entityCacheManager.InvalidateAll();
			throw;
		}
	}

	/// <inheritdoc />
	public override async Task SeedDataAsync(Type dataSeedProfileType, bool forceRun = false, CancellationToken cancellationToken = default)
	{
		try
		{
			if (_dbContext.Database.IsSqlServer())
			{
				await new DbLockedCriticalSection((SqlConnection)_dbContext.Database.GetDbConnection()).ExecuteActionAsync(DataSeedLockValue, async () =>
				{
					// podpora pro Connection Resiliency
					// seedování používá "Option 2 - Rebuild application state" popsanou v dokumentaci
					// viz: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
					var strategy = _dbContext.Database.CreateExecutionStrategy();
					await strategy.ExecuteInTransactionAsync(async _ =>
					{
						await base.SeedDataAsync(dataSeedProfileType, forceRun, cancellationToken).ConfigureAwait(false);
					},
					null,
					cancellationToken).ConfigureAwait(false);
				}, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				await base.SeedDataAsync(dataSeedProfileType, forceRun, cancellationToken).ConfigureAwait(false);
			}
		}
		catch
		{
			// Pokud neprošlo seedování celé, mohlo se stát, že proběhla jeho část v transakci a došlo k uložení části dat (později revertovaných)
			// do cache. Proto preventivně z cache odstraníme veškerá data (vzhledem ke kontextu použití této služby si to zřejmě můžeme dovolit).
			_entityCacheManager.InvalidateAll();
			throw;
		}
	}
}
