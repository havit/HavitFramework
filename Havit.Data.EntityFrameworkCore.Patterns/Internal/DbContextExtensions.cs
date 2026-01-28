using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Internal;

internal static class DbContextExtensions
{
	internal const int ContainsChunkSize = 5_000;

	internal static bool SupportsSqlServerOpenJson(this IDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		// Podmínka vychází z Microsoft.EntityFrameworkCore.SqlServer.Query.Internal.SqlServerQueryableMethodTranslatingExpressionVisitor,
		// metoda TranslatePrimitiveCollection.
		// https://github.com/dotnet/efcore/blob/931a67c0d6dc1738faf0b2ecf04f242a3789c4e7/src/EFCore.SqlServer/Query/Internal/SqlServerQueryableMethodTranslatingExpressionVisitor.cs#L127
		var sqlServerSingletonOptions = (dbContext as Microsoft.EntityFrameworkCore.DbContext)?.GetService<ISqlServerSingletonOptions>();

		return (sqlServerSingletonOptions == null)
			? false
			: (sqlServerSingletonOptions.EngineType == SqlServerEngineType.SqlServer) && (sqlServerSingletonOptions.SqlServerCompatibilityLevel >= 130);
	}

	internal static ParameterTranslationMode? GetParameterTranslationMode(this IDbContext dbContext)
	{
		RelationalOptionsExtension sqlServerOptionsExtension = null;
		var dboContextOptions = (dbContext as Microsoft.EntityFrameworkCore.DbContext)?.GetService<DbContextOptions>();
		if (dboContextOptions != null)
		{
			sqlServerOptionsExtension = RelationalOptionsExtension.Extract(dboContextOptions);
		}

		return (sqlServerOptionsExtension == null)
			? null
			: sqlServerOptionsExtension.ParameterizedCollectionMode;
	}

	/// <summary>
	/// Vrací true, pokud je potřeba použít chunking při načítání dat s podmínkou Contains (IN (...)).
	/// </summary>
	internal static bool ShouldUseChunkingForContainsCondition(this IDbContext _dbContext, int valuesToLoadCount, out int chunkSize)
	{
		// +--------------------------+-----------------------+-----------------------+--------------------------+-----------------------------------------+
		// | ParameterTranslationMode | OPENJSON 200 záznamů  | OPENJSON 5000 záznamů | bez openjson 200 záznamů | bez openjson 5000 záznamů               |
		// +--------------------------+-----------------------+-----------------------+--------------------------+-----------------------------------------+
		// | MultipleParameters       | IN (@p1, @p2, @p3...) | OPENJSON              | IN (@p1, @p2, @p3...)    | (*) IN (1, 2, 3...)                     |
		// | Parameter                | OPENJSON              | OPENJSON              | IN (@p1, @p2, @p3...)    | (*) IN (@p1, @p2, @p3...) --> exception |
		// | Contants                 | IN (1, 2, 3...)       | (*) IN (1, 2, 3...)   | IN (1, 2, 3...)          | (*) IN (1, 2, 3...)                     |
		// +--------------------------+-----------------------+-----------------------+--------------------------+-----------------------------------------+

		// Chunking potřebujeme pouze v situaci, kdy by bylo potřeba použít velký počet parametrů v IN (...) klauzuli (ev. došlo k pádu).
		// V tabulce vyznačeno pomocí (*).

		// Pokud se stane, že kolekce id obsahuje větší počet záznamů (než chunk size), ale ids jsou sekvencí,
		// budou data rozdělena na chunks a každý chunks se načte s jedním WHERE x >= min AND x <= max (bez použití IN (...)).
		// Považujeme to za okrajový scénář, který neřešíme.

		bool supportSqlServerOpenJson = _dbContext.SupportsSqlServerOpenJson();
		var parameterTranslationMode = _dbContext.GetParameterTranslationMode();

		chunkSize = ContainsChunkSize;

		// Pokud není OPENJSON a je použit ParameterTranslationMode.Parameter, EF Core
		// použije IN (@p1, @p2, @p3) bez ohledu na počet parametrů. Potřebujeme použít maximálně 2100 parametrů.
		if (!supportSqlServerOpenJson && (parameterTranslationMode == ParameterTranslationMode.Parameter))
		{
			chunkSize = Math.Min(2000, chunkSize); // načítáme po částech
		}

		return (valuesToLoadCount > chunkSize) // velikost dat překročila velikost chunku (tedy sloupce v tabulce výše s 5000 záznamy)
			&& (!supportSqlServerOpenJson  // sloupec "bez openjson"
				|| (parameterTranslationMode == ParameterTranslationMode.Constant)); // poslední řádek

	}
}
