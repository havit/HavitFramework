using System;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections.StoredProcedures;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// <para>Umožňuje nastaviť špecifickú konfiguráciu pre DbInjections na <see cref="DbContextOptions"/>.</para>
    /// <para>Inštancie tejto triedy vznikajú pomocou extension metódy <see cref="DbInjectionsExtensions.UseDbInjections"/>. Táto trieda nie je navrhnutá tak, aby jej inštancie boli priamo vytvárané v aplikačnom kóde.</para>
    /// 
    /// </summary>
    public class DbInjectionsExtensionBuilder : IDbInjectionsExtensionBuilderInfrastructure
	{
		/// <summary>
        /// Konstruktor.
        /// </summary>
		public DbInjectionsExtensionBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            OptionsBuilder = optionsBuilder;
        }

		/// <summary>
		/// Zapne podporu pre <see cref="IDbInjector"/> objekty pre uložené procedúry. Umožňuje automaticky spravovať uložené procedúry pomocou migrácii.
		/// </summary>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder UseStoredProcedures()
			=> WithOption(e => e
                .WithAnnotationProvider<StoredProcedureAnnotationProvider>()
                .WithSqlGenerator<StoredProcedureSqlGenerator>());

		/// <summary>
		/// Umožňuje vypnúť alebo zapnúť odstraňovanie duplicitných párov aktuálnych a starých anotácii v prípade AlterDatabaseOperation.
		/// Štandardne je táto funkcionalita zapnutá.
		/// </summary>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder ConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel) =>
			WithOption(e => e.WithConsolidateStatementsForMigrationsAnnotationsForModel(consolidateStatementsForMigrationsAnnotationsForModel));

		/// <summary>
		/// WORK IN PROGRESS: Zapne podporu pre <see cref="IDbInjector"/> objekty pre pohľady. Umožňuje automaticky spravovať pohľady pomocou migrácii.
		/// </summary>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder UseViews()
		{
			return WithOption(e => e.WithAnnotationProvider<ViewAnnotationProvider>()
                .WithSqlGenerator<ViewSqlGenerator>());
		}

        /// <summary>
        /// Gets the core options builder.
        /// </summary>
        protected virtual DbContextOptionsBuilder OptionsBuilder { get; }

        DbContextOptionsBuilder IDbInjectionsExtensionBuilderInfrastructure.OptionsBuilder => OptionsBuilder;

        /// <summary>
        /// Sets an option by cloning the extension used to store the settings. This ensures the builder
        /// does not modify options that are already in use elsewhere.
        /// </summary>
        protected virtual DbInjectionsExtensionBuilder WithOption(Func<DbInjectionsExtension, DbInjectionsExtension> setAction)
        {
            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
                setAction(OptionsBuilder.Options.FindExtension<DbInjectionsExtension>() ?? new DbInjectionsExtension()));

            return this;
        }
    }
}