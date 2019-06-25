using System;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
// Správny namespace je Microsoft.EntityFrameworkCore! Konvencia Microsoftu.
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Havit.Data.EntityFrameworkCore.Migrations extension methods for <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    public static class ExtendedMigrationsDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Registruje služby používané podporou pre Extended Migrations. Pomocou <paramref name="setupAction"/> je možné aktivovať rôzne funkčnosti Extended Migrations.
        /// </summary>
        public static DbContextOptionsBuilder UseExtendedMigrations(this DbContextOptionsBuilder optionsBuilder, Action<ExtendedMigrationsExtensionBuilder> setupAction = null)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);

            IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

            var annotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>();
            if (annotationProviderExtension == null)
            {
                throw new InvalidOperationException("Necessary extension (CompositeMigrationsAnnotationProviderExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
            }

            builder.AddOrUpdateExtension(annotationProviderExtension.WithAnnotationProvider<DbInjectionsMigrationsAnnotationProvider>());

            var sqlGeneratorExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>();
            if (sqlGeneratorExtension == null)
            {
                throw new InvalidOperationException("Necessary extension (CompositeMigrationsSqlGeneratorExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
            }
            builder.AddOrUpdateExtension(sqlGeneratorExtension.WithGeneratorType<DbInjectionMigrationOperationSqlGenerator>());

            setupAction?.Invoke(new ExtendedMigrationsExtensionBuilder(optionsBuilder));

            return optionsBuilder;
        }
    }
}