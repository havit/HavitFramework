using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
    /// <summary>
    /// Bázová trieda pre <see cref="DbContext"/> používaný v Business Layer projektoch. Mal by sa používať výhradne pre správu schémy DB.
    /// 
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
				UseStringPropertiesDefaultValueConvention = true
			};
		}

		/// <inheritdoc />
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseModelExtensions(builder => builder
				.UseStoredProcedures()
				.UseExtendedProperties()
				.UseBusinessLayerStoredProcedures()
				.UseViews());
			optionsBuilder.UseSqlServerExtendedProperties();

			optionsBuilder.ConditionalyUseConventionSetPlugin<CollectionExtendedPropertiesConventionPlugin>(() => Settings.UseCollectionExtendedPropertiesConvention);
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
			optionsBuilder.ConditionalyUseConventionSetPlugin<IndexForForeignKeysConventionPlugin>(() => Settings.UseIndexForForeignKeysConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<IndexForLanguageUiCulturePropertyConventionPlugin>(() => Settings.UseIndexForLanguageUiCulturePropertyConvention);
			optionsBuilder.ConditionalyUseConventionSetPlugin<IndexForLocalizationTableConventionPlugin>(() => Settings.UseIndexForLocalizationTableConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<IndexNamingConventionPlugin>(() => Settings.UseIndexNamingConvention);

			optionsBuilder.ConditionalyUseConventionSetPlugin<XmlCommentsForDescriptionPropertyConventionPlugin>(() => Settings.UseXmlCommentsForDescriptionPropertyConvention);
		}

		/// <inheritdoc />
		protected override void ModelCreatingCompleting(ModelBuilder modelBuilder)
		{
			modelBuilder.ForSqlServerExtendedPropertiesAttributes();
			base.ModelCreatingCompleting(modelBuilder);
		}

		///// <inheritdoc />
		//protected override IEnumerable<IModelConvention> GetModelConventions()
		//{
		//	foreach (var convention in base.GetModelConventions())
		//	{
		//		yield return convention;
		//	}

		//	yield return new PrefixedTablePrimaryKeysConvention();
		//	yield return new ForeignKeysColumnNamesConvention();
		//	yield return new LocalizationTablesParentEntitiesConvention();
		//	yield return new DefaultValueSqlAttributeConvention();
		//	yield return new DefaultValueAttributeConvention();
		//	yield return new StringPropertiesDefaultValueConvention();
		//	yield return new NamespaceExtendedPropertyConvention();
		//	yield return new CollectionExtendedPropertiesConvention();
		//	yield return new XmlCommentsForDescriptionPropertyConvention();
		//	yield return new BusinessLayerIndexesConventions();
  //          yield return new CharColumnTypeForCharPropertyConvention();
		//}

		/// <summary>
		/// Registruje <see cref="IModelExtender"/>y z <paramref name="extendersAssembly"/>. Vyžaduje, aby v DbContexte bola zaregistrovaná služba <see cref="IModelExtensionAnnotationProvider"/> (štandardne je registrovaná v <see cref="OnConfiguring"/>.
		/// </summary>
	    protected void RegisterModelExtensions(ModelBuilder modelBuilder, Assembly extendersAssembly = default)
	    {
            modelBuilder.ForModelExtensions(this.GetService<IModelExtensionAnnotationProvider>(), extendersAssembly ?? Assembly.GetCallingAssembly());
	    }
	}
}
