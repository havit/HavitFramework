using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// Extension metódy pre registráciu infraštruktúrnych služieb používané podporou pre Extended Migrations.
    /// </summary>
    public static class InfrastructureExtensions
	{
		/// <summary>
		/// Registruje infraštruktúrne služby používané podporou pre Extended Migrations.
		/// </summary>
		public static void UseExtendedMigrationsInfrastructure(this DbContextOptionsBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(new CompositeMigrationsAnnotationProviderExtension().WithAnnotationProvider<SqlServerMigrationsAnnotationProvider>());
			builder.AddOrUpdateExtension(new CompositeMigrationsSqlGeneratorExtension());
		}
	}
}