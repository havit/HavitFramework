﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Internal;

/// <summary>
/// Zajistí použití DbLockedMigratoru pro provedení migrace schématu databáze pod databázovým zámkem, čímž zajistíme thread safe běh databázových migrací.
/// </summary>
public class DbLockedMigratorInstallerExtension : IDbContextOptionsExtension
{
	private DbContextOptionsExtensionInfo _info;

	/// <inheritdoc />
	public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		services.Replace(ServiceDescriptor.Transient<Microsoft.EntityFrameworkCore.Migrations.IMigrator, Migrations.Internal.DbLockedMigrator>());
	}

	/// <inheritdoc />
	public void Validate(IDbContextOptions options)
	{
		// NOOP
	}

	private class ExtensionInfo : DbContextOptionsExtensionInfo
	{
		private string _logFragment;

		public ExtensionInfo(IDbContextOptionsExtension extension)
			: base(extension)
		{
		}

		public override bool IsDatabaseProvider => false;

		public override string LogFragment => _logFragment ??= "using " + typeof(DbLockedMigrator).FullName;

		public override int GetServiceProviderHashCode() => 0x648;

		public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
		{
		}

		public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
		{
			return (other is ExtensionInfo);
		}
	}
}
