using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore;

/// <summary>
/// Extension metody k DbContextOptionsBuilder.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
	/// <summary>
	/// Zajistí použití migrátoru chráněného databázovým zámkem před paralelním spuštěním.
	/// </summary>
	public static DbContextOptionsBuilder UseDbLockedMigrator(this DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.Options.FindExtension<DbLockedMigratorInstallerExtension>() == null)
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new DbLockedMigratorInstallerExtension());
		}
		return optionsBuilder;
	}
}

