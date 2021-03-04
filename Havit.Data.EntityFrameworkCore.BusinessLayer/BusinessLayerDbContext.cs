using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
    /// <summary>
    /// Bázová trieda pre <see cref="DbContext"/> používaný v Business Layer projektoch. Mal by sa používať výhradne pre správu schémy DB.
    /// <remarks>
    /// Pridáva podporu pre extended properties a DB Injections a ich spoločnú infraštruktúru. Definuje rôzne konvencie používané na Business Layer projektoch.
    /// </remarks>
    /// </summary>
    public abstract class BusinessLayerDbContext : DbContext
	{
		protected new virtual BusinessLayerDbContextSettings Settings => (BusinessLayerDbContextSettings)base.Settings;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext()
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext(DbContextOptions options)
			: base(options)
		{
		}

		/// <inheritdoc />
		protected override DbContextSettings CreateDbContextSettings()
		{
			return new BusinessLayerDbContextSettings
			{
				UseStringPropertiesDefaultValueConvention = true,
                ModelExtensionsAssembly = GetType().Assembly
			};
		}

		/// <inheritdoc />
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseModelExtensions(builder => builder
						 .ModelExtensionsAssembly(Settings.ModelExtensionsAssembly)
				.UseStoredProcedures()
				.UseExtendedProperties()
				.UseBusinessLayerStoredProcedures()
				.UseViews());
			optionsBuilder.UseSqlServerExtendedProperties();

			optionsBuilder.ConditionalyUseConventionSetPlugin<CollectionExtendedPropertiesConventionPlugin>(() => Settings.UseCollectionExtendedPropertiesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<CollectionOrderIndexConventionPlugin>(() => Settings.UseCollectionOrderIndexConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<ExtendedPropertiesConventionPlugin>(() => Settings.UseAttributeBasedExtendedPropertiesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<DefaultValueAttributeConventionPlugin>(() => Settings.UseDefaultValueAttributeConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<DefaultValueSqlAttributeConventionPlugin>(() => Settings.UseDefaultValueSqlAttributeConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<CharColumnTypeForCharPropertyConventionPlugin>(() => Settings.UseCharColumnTypeForCharPropertyConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<NamespaceExtendedPropertyConventionPlugin>(() => Settings.UseNamespaceExtendedPropertyConvention);

			// reagují na přidání cizího klíče, naše konvence musíme dostat před tvorbu indexů vestavěnou v EF Core
			optionsBuilder.ConditionalyUseConventionSetPlugin<ForeignKeysColumnNamesConventionPlugin>(() => Settings.UseForeignKeysColumnNamesConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<PrefixedTablePrimaryKeysConventionPlugin>(() => Settings.UsePrefixedTablePrimaryKeysConvention);
			// konvence používá primární klíč nastavený v předchozí konvenci
			optionsBuilder.ConditionalyUseConventionSetPlugin<LocalizationTablesParentEntitiesConventionPlugin>(() => Settings.UseLocalizationTablesParentEntitiesConvention);
			// reagují na přidání cizího klíče, konvenci musíme dostat před tvorbu indexů vestavěnou v EF Core, ale až za naše předchozí konvence
			optionsBuilder.ConditionalyUseConventionSetPlugin<ForeignKeysIndexConventionPlugin>(() => Settings.UseForeignKeysIndexConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<LanguageUiCultureIndexConventionPlugin>(() => Settings.UseLanguageUiCultureIndexConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<LocalizationTableIndexConventionPlugin>(() => Settings.LocalizationTableIndexConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<XmlCommentsForDescriptionPropertyConventionPlugin>(() => Settings.UseXmlCommentsForDescriptionPropertyConvention);
		}
	}
}
