using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
	/// <summary>
	/// Bázová trieda pre <see cref="DbContext"/> používaný v Business Layer projektoch. Mal by sa používať výhradne pre správu schémy DB.
	/// </summary>
	/// <remarks>
	/// Pridáva podporu pre extended properties a DB Injections a ich spoločnú infraštruktúru. Definuje rôzne konvencie používané na Business Layer projektoch.
	/// </remarks>
	public abstract class BusinessLayerDbContext : DbContext
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext() : this(new DbContextOptions<BusinessLayerDbContext>())
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext(DbContextOptions options)
			: base(options)
		{
		}

		/// <summary>
		/// Vrací konfiguraci konvencí pro BusinessLayerDbContextu.
		/// </summary>
		protected virtual BusinessLayerDbContextSettings CreateDbContextSettings()
		{
			return new BusinessLayerDbContextSettings
			{
				ModelExtensionsAssembly = GetType().Assembly
			};
		}

		/// <inheritdoc />
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			// Sice není ideální ve frameworku použít OnConfiguring, nicméně použití BusinessLayerDbContextu se nepředpokládá tam, kde toto bude blokující pro použití DbContextu (např. pooled db connection)
			BusinessLayerDbContextSettings settings = CreateDbContextSettings();

			optionsBuilder.UseFrameworkConventions(frameworkConventions => frameworkConventions.UseStringPropertiesDefaultValueConvention(true));

			optionsBuilder.UseModelExtensions(builder => builder
						 .ModelExtensionsAssembly(settings.ModelExtensionsAssembly)
				.UseStoredProcedures()
				.UseExtendedProperties()
				.UseBusinessLayerStoredProcedures()
				.UseViews());
			optionsBuilder.UseSqlServerExtendedProperties();


			optionsBuilder.ConditionalyUseConventionSetPlugin<CollectionExtendedPropertiesConventionPlugin>(() => settings.UseCollectionExtendedPropertiesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<CollectionOrderIndexConventionPlugin>(() => settings.UseCollectionOrderIndexConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<ExtendedPropertiesConventionPlugin>(() => settings.UseAttributeBasedExtendedPropertiesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<DefaultValueAttributeConventionPlugin>(() => settings.UseDefaultValueAttributeConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<DefaultValueSqlAttributeConventionPlugin>(() => settings.UseDefaultValueSqlAttributeConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<CharColumnTypeForCharPropertyConventionPlugin>(() => settings.UseCharColumnTypeForCharPropertyConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<NamespaceExtendedPropertyConventionPlugin>(() => settings.UseNamespaceExtendedPropertyConvention);

			// reagují na přidání cizího klíče, naše konvence musíme dostat před tvorbu indexů vestavěnou v EF Core
			optionsBuilder.ConditionalyUseConventionSetPlugin<ForeignKeysColumnNamesConventionPlugin>(() => settings.UseForeignKeysColumnNamesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<PrefixedTablePrimaryKeysConventionPlugin>(() => settings.UsePrefixedTablePrimaryKeysConvention);
			// konvence používá primární klíč nastavený v předchozí konvenci
			optionsBuilder.ConditionalyUseConventionSetPlugin<LocalizationTablesParentEntitiesConventionPlugin>(() => settings.UseLocalizationTablesParentEntitiesConvention);
			// reagují na přidání cizího klíče, konvenci musíme dostat před tvorbu indexů vestavěnou v EF Core, ale až za naše předchozí konvence
			optionsBuilder.ConditionalyUseConventionSetPlugin<ForeignKeysIndexConventionPlugin>(() => settings.UseForeignKeysIndexConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<LanguageUiCultureIndexConventionPlugin>(() => settings.UseLanguageUiCultureIndexConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<LocalizationTableIndexConventionPlugin>(() => settings.LocalizationTableIndexConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<XmlCommentsForDescriptionPropertyConventionPlugin>(() => settings.UseXmlCommentsForDescriptionPropertyConvention);
		}
	}
}
