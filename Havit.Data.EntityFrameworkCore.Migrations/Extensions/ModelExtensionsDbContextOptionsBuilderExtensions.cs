using System;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
// Správny namespace je Microsoft.EntityFrameworkCore! Konvencia Microsoftu.
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Havit.Data.EntityFrameworkCore.Migrations extension methods for <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    public static class ModelExtensionsDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Registruje služby používané podporou pre Model Extensions.
        ///
        /// <para>V štandardnom nastavení zapne podporu pre správu uložených procedúr a pohľadov pomocou migrácii.</para>
        /// </summary>
        public static DbContextOptionsBuilder UseModelExtensions(this DbContextOptionsBuilder optionsBuilder)
            => UseModelExtensions(optionsBuilder, builder => builder.UseStoredProcedures().UseViews());

        /// <summary>
        /// Registruje služby používané podporou pre Model Extensions. Pomocou <paramref name="setupAction"/> je možné aktivovať rôzne funkčnosti Model Extensions.
        /// </summary>
        public static DbContextOptionsBuilder UseModelExtensions(
			this DbContextOptionsBuilder optionsBuilder,
			Action<ModelExtensionsExtensionBuilder> setupAction)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);
            Contract.Requires<ArgumentNullException>(setupAction != null);

            optionsBuilder.UseExtendedMigrationsInfrastructure();

            IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

            var migrationsAnnotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>();
            if (migrationsAnnotationProviderExtension == null)
            {
                throw new InvalidOperationException("Necessary extension (CompositeMigrationsAnnotationProviderExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
            }

            builder.AddOrUpdateExtension(migrationsAnnotationProviderExtension.WithAnnotationProvider<ModelExtensionsMigrationsAnnotationProvider>());

            var relationalAnnotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeRelationalAnnotationProviderExtension>();
            if (relationalAnnotationProviderExtension == null)
            {
	            throw new InvalidOperationException("Necessary extension (CompositeRelationalAnnotationProviderExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
            }

            builder.AddOrUpdateExtension(relationalAnnotationProviderExtension.WithAnnotationProvider<ModelExtensionsRelationalAnnotationProvider>());

            var sqlGeneratorExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>();
            if (sqlGeneratorExtension == null)
            {
                throw new InvalidOperationException("Necessary extension (CompositeMigrationsSqlGeneratorExtension) not found, please make sure infrastructure has been registered into DbContextOptionsBuilder (extension method UseCodeMigrationsInfrastructure on this object is called)");
            }
            builder.AddOrUpdateExtension(sqlGeneratorExtension.WithGeneratorType<ModelExtensionMigrationOperationSqlGenerator>());

            setupAction?.Invoke(new ModelExtensionsExtensionBuilder(optionsBuilder));

            return optionsBuilder;
        }
    }
}