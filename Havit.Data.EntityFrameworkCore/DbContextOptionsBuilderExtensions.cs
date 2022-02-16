using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Extension metody k DbContextOptionsBuilder.
	/// </summary>
	public static class DbContextOptionsBuilderExtensions
	{
		/// <summary>
		/// Zajistí použití frameworkových konvencí.
		/// </summary>
		public static DbContextOptionsBuilder UseFrameworkConventions(this DbContextOptionsBuilder optionsBuilder)
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new FrameworkConventionSetOptionsExtension());

			return optionsBuilder;
		}

		/// <summary>
		/// Zajistí použití migrátoru chráněného databázovým zámkem před paralelním spuštěním.
		/// </summary>
		public static DbContextOptionsBuilder UseDbLockedMigrator(this DbContextOptionsBuilder optionsBuilder)
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(optionsBuilder.Options.FindExtension<DbLockedMigratorInstallerExtension>() ?? new DbLockedMigratorInstallerExtension());
			return optionsBuilder;
		}
	}
}

