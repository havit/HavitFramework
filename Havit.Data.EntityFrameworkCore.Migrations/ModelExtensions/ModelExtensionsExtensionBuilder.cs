using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

#pragma warning disable 419
/// <summary>
/// <para>Umožňuje nastaviť špecifickú konfiguráciu pre Model Extensions na <see cref="DbContextOptions"/>.</para>
/// <para>Inštancie tejto triedy vznikajú pomocou extension metódy <see cref="ModelExtensionsDbContextOptionsBuilderExtensions.UseModelExtensions"/>. Táto trieda nie je navrhnutá tak, aby jej inštancie boli priamo vytvárané v aplikačnom kóde.</para>
/// </summary>
public class ModelExtensionsExtensionBuilder : IModelExtensionsExtensionBuilderInfrastructure
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ModelExtensionsExtensionBuilder(DbContextOptionsBuilder optionsBuilder)
	{
		OptionsBuilder = optionsBuilder;
	}

	/// <summary>
	/// Zapne podporu pre <see cref="IModelExtender"/> objekty pre uložené procedúry. Umožňuje automaticky spravovať uložené procedúry pomocou migrácii.
	/// </summary>
	/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
	public ModelExtensionsExtensionBuilder UseStoredProcedures()
		=> WithOption(e => e
			.WithAnnotationProvider<StoredProcedureAnnotationProvider>()
			.WithSqlGenerator<StoredProcedureSqlGenerator>());

	/// <summary>
	/// Umožňuje vypnúť alebo zapnúť odstraňovanie duplicitných párov aktuálnych a starých anotácii v prípade AlterDatabaseOperation.
	/// Štandardne je táto funkcionalita zapnutá.
	/// </summary>
	/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
	public ModelExtensionsExtensionBuilder ConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel) =>
		WithOption(e => e.WithConsolidateStatementsForMigrationsAnnotationsForModel(consolidateStatementsForMigrationsAnnotationsForModel));

	/// <summary>
	/// WORK IN PROGRESS: Zapne podporu pre <see cref="IModelExtender"/> objekty pre pohľady. Umožňuje automaticky spravovať pohľady pomocou migrácii.
	/// </summary>
	/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
	public ModelExtensionsExtensionBuilder UseViews()
	{
		return WithOption(e => e.WithAnnotationProvider<ViewAnnotationProvider>()
			.WithSqlGenerator<ViewSqlGenerator>());
	}

	/// <summary>
	/// Gets the core options builder.
	/// </summary>
	protected virtual DbContextOptionsBuilder OptionsBuilder { get; }

	DbContextOptionsBuilder IModelExtensionsExtensionBuilderInfrastructure.OptionsBuilder => OptionsBuilder;

	/// <summary>
	/// Sets an option by cloning the extension used to store the settings. This ensures the builder
	/// does not modify options that are already in use elsewhere.
	/// </summary>
	protected virtual ModelExtensionsExtensionBuilder WithOption(Func<ModelExtensionsExtension, ModelExtensionsExtension> setAction)
	{
		((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
			setAction(OptionsBuilder.Options.FindExtension<ModelExtensionsExtension>() ?? new ModelExtensionsExtension()));

		return this;
	}

	/// <summary>
	/// Nastaví <see cref="Assembly"/>, ktorá obsahuje <see cref="IModelExtender"/> objekty.
	/// </summary>
	/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
	public ModelExtensionsExtensionBuilder ModelExtensionsAssembly(Assembly modelExtensionsAssembly)
	{
		Contract.Requires<ArgumentNullException>(modelExtensionsAssembly != null);

		return WithOption(e => e.WithExtensionsAssembly(modelExtensionsAssembly));
	}
}