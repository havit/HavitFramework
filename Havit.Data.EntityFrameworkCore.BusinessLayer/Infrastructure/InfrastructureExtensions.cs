using System;
using Havit.Diagnostics.Contracts;
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
		public static void UseCodeMigrationsInfrastructure(this IDbContextOptionsBuilderInfrastructure optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			optionsBuilder.AddOrUpdateExtension(new CompositeMigrationsAnnotationProviderExtension().WithAnnotationProvider<SqlServerMigrationsAnnotationProvider>());
			optionsBuilder.AddOrUpdateExtension(new CompositeMigrationsSqlGeneratorExtension());
		}
	}
}