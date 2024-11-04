using Havit.Data.EntityFrameworkCore.Threading.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Havit.Data.EntityFrameworkCore.Migrations.Internal;

/// <summary>
/// Migrator databáze rozšířený o použití databázového zámku. Zajišťuje "thread safe" migrace databázového schématu v případě paralelního běhu (např. start aplikace v clusteru, případně z více různých aplikací).
/// </summary>
public class DbLockedMigrator : Migrator
{
	private const string EfCoreMigrationsLockValue = "EF_Core_Migrations";

	private readonly IDatabaseCreator _databaseCreator;
	private readonly IRelationalConnection _connection;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbLockedMigrator(IMigrationsAssembly migrationsAssembly,
		IHistoryRepository historyRepository,
		IDatabaseCreator databaseCreator,
		IMigrationsSqlGenerator migrationsSqlGenerator,
		IRawSqlCommandBuilder rawSqlCommandBuilder,
		IMigrationCommandExecutor migrationCommandExecutor,
		IRelationalConnection connection,
		ISqlGenerationHelper sqlGenerationHelper,
		ICurrentDbContext currentContext,
		IModelRuntimeInitializer modelRuntimeInitializer,
		IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
		IRelationalCommandDiagnosticsLogger commandLogger,
		IDatabaseProvider databaseProvider,
		IMigrationsModelDiffer migrationsModelDiffer,
		IDesignTimeModel designTimeModel,
		IDbContextOptions dbContextOptions,
		IExecutionStrategy executionStrategy)
		: base(migrationsAssembly,
			historyRepository,
			databaseCreator,
			migrationsSqlGenerator,
			rawSqlCommandBuilder,
			migrationCommandExecutor,
			connection,
			sqlGenerationHelper,
			currentContext,
			modelRuntimeInitializer,
			logger,
			commandLogger,
			databaseProvider,
			migrationsModelDiffer,
			designTimeModel,
			dbContextOptions,
			executionStrategy)
	{
		this._databaseCreator = databaseCreator;
		this._connection = connection;
	}

	/// <inheritdoc />
	public override void Migrate(string targetMigration = null)
	{
		// Databáze ještě nemusí ani existovat, pak se nám nepodaří připojit pomocí connection stringu s názvem databáze.
		// Proto musíme nejprve databázi založit.
		// Tím ovšem přesuneme úzké místo sem, mezi Exists/Create.
		// To se pokusíme kompenzovat tak, že ignorujeme výjimku o existenci databáze.
		IRelationalDatabaseCreator relationalDatabaseCreator = (IRelationalDatabaseCreator)_databaseCreator;
		if (!relationalDatabaseCreator.Exists())
		{
			try
			{
				relationalDatabaseCreator.Create();
			}
			catch (Exception e) when (e is Microsoft.Data.SqlClient.SqlException sqlException && sqlException.Number == 1801 /* https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/cc645860(v=sql.105 */)
			{
				// NOOP
			}
		}

		// Databáze je založena, můžeme použít bezpečně použít dodaný connection string.
		new DbLockedCriticalSection((SqlConnection)_connection.DbConnection).ExecuteAction(EfCoreMigrationsLockValue, () =>
		{
			base.Migrate(targetMigration);
		});
	}

	/// <inheritdoc />
	public override async Task MigrateAsync(string targetMigration = null, CancellationToken cancellationToken = default)
	{
		IRelationalDatabaseCreator relationalDatabaseCreator = (IRelationalDatabaseCreator)_databaseCreator;
		if (!await relationalDatabaseCreator.ExistsAsync(cancellationToken).ConfigureAwait(false))
		{
			try
			{
				await relationalDatabaseCreator.CreateAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception e) when ((e is Microsoft.Data.SqlClient.SqlException sqlException) && (sqlException.Number == 1801 /* https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/cc645860(v=sql.105 */))
			{
				// NOOP
			}
		}

		// Databáze je založena, můžeme použít bezpečně použít dodaný connection string.
		await new DbLockedCriticalSection((SqlConnection)_connection.DbConnection).ExecuteActionAsync(EfCoreMigrationsLockValue, async () =>
		{
			await base.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
		}, cancellationToken).ConfigureAwait(false);
	}
}
#pragma warning restore EF1001 // Internal EF Core API usage.
