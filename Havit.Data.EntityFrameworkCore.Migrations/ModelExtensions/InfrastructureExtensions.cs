using System;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
    /// <summary>
    /// Extension metódy pre registráciu Extended Migrations infraštruktúrnych služieb používané podporou Model Extensions.
    /// </summary>
    public static class InfrastructureExtensions
	{
        /// <summary>
        /// Registruje Extended Migrations infraštruktúrne služby používané podporou pre Model Extensions.
        /// </summary>
        public static void UseExtendedMigrationsInfrastructure(this DbContextOptionsBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

            // handle multiple invocations of UseExtendedMigrationsInfrastructure
            var compositeMigrationsAnnotationProviderExtension =
                optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>() ??
                new CompositeMigrationsAnnotationProviderExtension();
            builder.AddOrUpdateExtension(compositeMigrationsAnnotationProviderExtension);

            var compositeMigrationsSqlGeneratorExtension =
                optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>() ??
                new CompositeMigrationsSqlGeneratorExtension();
            builder.AddOrUpdateExtension(compositeMigrationsSqlGeneratorExtension);
		}
	}
}