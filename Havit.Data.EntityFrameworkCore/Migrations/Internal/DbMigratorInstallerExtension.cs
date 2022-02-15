using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Migrations.Internal
{
	internal class DbMigratorInstallerExtension : IDbContextOptionsExtension
	{
		private DbContextOptionsExtensionInfo _info;

		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

		/// <inheritdoc />
		public void ApplyServices(IServiceCollection services)
		{
			services.Replace(ServiceDescriptor.Transient<Microsoft.EntityFrameworkCore.Migrations.IMigrator, Migrations.Internal.DbMigrator>());
		}

		/// <inheritdoc />
		public void Validate(IDbContextOptions options)
		{
			// NOOP
		}

		protected class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			private string _logFragment;

			public ExtensionInfo(IDbContextOptionsExtension extension)
				: base(extension)
			{
			}

			public override bool IsDatabaseProvider => false;

			public override string LogFragment => _logFragment ??= "using " + typeof(DbMigrator).FullName;

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

}
