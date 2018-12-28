using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	/// <summary>
	/// <para>Umožňuje nastaviť špecifickú konfiguráciu pre DbInjections na <see cref="DbContextOptions"/>.</para>
	/// <para>Inštancie tejto triedy vznikajú pomocou extension metódy <see cref="DbInjectionsExtensions.UseDbInjections"/>. Táto trieda nie je navrhnutá tak, aby jej inštancie boli priamo vytvárané v aplikačnom kóde.</para>
	/// 
	/// </summary>
	public class DbInjectionsExtensionBuilder : DbInjectionsExtensionBuilderBase<DbInjectionsExtensionBuilder>
	{
		/// <inheritdoc />
		public DbInjectionsExtensionBuilder(DbContextOptionsBuilder optionsBuilder) 
			: base(optionsBuilder)
		{
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
		/// Zapne podporu pre extended properties na <see cref="IDbInjector"/> objektoch. Umožňuje automaticky spravovať extended properties pre objekty DbInjections pomocou migrácii.
		/// </summary>
		/// <remarks>
		/// Je nutné odekorovať <see cref="IDbInjector"/> objekty pomocou atribútov dediacich z <see cref="DbInjectionExtendedPropertiesAttribute"/>. Podporované sú len tie objekty v DB, na ktoré je možné pridať extended properties v SQL Serveri.
		/// </remarks>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder UseExtendedProperties() => 
			WithOption(e => e.WithAnnotationProvider<ExtendedPropertiesAnnotationProvider>());

		/// <summary>
		/// Zapne podporu pre rozšírenia uložených procedúr podporovaných Business Layerom (resp. jeho generátorom). Jedná sa primárne o (polo)automatické generovanie extended properties pre uložené procedúry ako napr. MS_Description (z XML komentáru) alebo Attach (z Attach atribútu).
		/// </summary>
		/// <remarks>
		/// Pre podporu nastavenia MS_Description extended property je nutné zapnúť generovanie dokumentačného XML súboru z XML komentárov na projekte, kde sa DbInjectory uložených procedúr nachádzajú.
		/// </remarks>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder UseBusinessLayerStoredProcedures() =>
			WithOption(e => e
				.WithAnnotationProvider<StoredProcedureAttachPropertyAnnotationProvider>()
				.WithAnnotationProvider<StoredProcedureMsDescriptionPropertyAnnotationProvider>());

		/// <summary>
		/// Umožňuje vypnúť alebo zapnúť odstraňovanie duplicitných párov aktuálnych a starých anotácii v prípade AlterDatabaseOperation.
		/// Štandardne je táto funkcionalita zapnutá.
		/// </summary>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder ConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel) =>
			WithOption(e => e.WithConsolidateStatementsForMigrationsAnnotationsForModel(consolidateStatementsForMigrationsAnnotationsForModel));

		/// <summary>
		/// WORK IN PROGRESS: Zapne podporu pre <see cref="IDbInjector"/> objekty pre pohľady. Umožňuje automaticky spravovať uložené procedúry pomocou migrácii.
		/// </summary>
		/// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public DbInjectionsExtensionBuilder UseViews()
		{
			return WithOption(e => e.WithAnnotationProvider<ViewAnnotationProvider>()
				.WithSqlGenerator<ViewSqlGenerator>());
		}
	}
}