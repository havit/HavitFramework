using System;
using System.Collections.Generic;
using System.Text;
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
	/// Zajistí použití frameworkových konvencí.
	/// </summary>
	public static DbContextOptionsBuilder UseFrameworkConventions(this DbContextOptionsBuilder optionsBuilder, Action<FrameworkConventionSetOptionsBuilder> frameworkConventionSetOptionsBuilder = null)
	{
		if (optionsBuilder.Options.FindExtension<FrameworkConventionSetOptionsExtension>() == null)
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new FrameworkConventionSetOptionsExtension());
		}

		frameworkConventionSetOptionsBuilder?.Invoke(new FrameworkConventionSetOptionsBuilder(optionsBuilder));

		return optionsBuilder;
	}

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

