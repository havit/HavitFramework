using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	/// <summary>
	/// Extension metódy pre registráciu infraštruktúrnych služieb používaných podporou pre extended properties a DB Injections.
	/// </summary>
	public static class InfrastructureExtensions
	{
		/// <summary>
		/// Registruje infraštruktúrne služby používané podporou pre extended properties a DB Injections.
		/// </summary>
		public static void UseCodeMigrationsInfrastructure(this DbContextOptionsBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(new CompositeMigrationsAnnotationProviderExtension().WithAnnotationProvider<SqlServerMigrationsAnnotationProvider>());
			builder.AddOrUpdateExtension(new CompositeMigrationsSqlGeneratorExtension());
		}
	}
}