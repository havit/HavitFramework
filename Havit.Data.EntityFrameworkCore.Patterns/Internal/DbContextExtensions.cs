using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Internal;

internal static class DbContextExtensions
{
	internal static bool SupportsSqlServerOpenJson(this IDbContext dbContext)
	{
		ArgumentNullException.ThrowIfNull(dbContext);

		// Podmínka vychází z Microsoft.EntityFrameworkCore.SqlServer.Query.Internal.SqlServerQueryableMethodTranslatingExpressionVisitor,
		// metoda TranslatePrimitiveCollection.
		// https://github.com/dotnet/efcore/blob/931a67c0d6dc1738faf0b2ecf04f242a3789c4e7/src/EFCore.SqlServer/Query/Internal/SqlServerQueryableMethodTranslatingExpressionVisitor.cs#L127
		var sqlServerSingletonOptions = (dbContext as Microsoft.EntityFrameworkCore.DbContext)?.GetService<ISqlServerSingletonOptions>();

		// v EF Core 10 potřebujeme test rozšířit o ověření nastavení ParameterizedCollectionMode)
		RelationalOptionsExtension sqlServerOptionsExtension = null;
		var dboContextOptions = (dbContext as Microsoft.EntityFrameworkCore.DbContext)?.GetService<DbContextOptions>();
		if (dboContextOptions != null)
		{
			sqlServerOptionsExtension = RelationalOptionsExtension.Extract(dboContextOptions);
		}

		return (sqlServerSingletonOptions == null) || (sqlServerOptionsExtension == null)
			? false
			: sqlServerSingletonOptions.SqlServerCompatibilityLevel >= 130 && (sqlServerOptionsExtension.ParameterizedCollectionMode == ParameterTranslationMode.Parameter);

	}
}
