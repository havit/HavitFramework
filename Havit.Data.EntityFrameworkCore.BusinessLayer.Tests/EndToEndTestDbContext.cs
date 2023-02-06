using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
	public class EndToEndTestDbContext : TestDbContext
	{
		private readonly Action<ModelBuilder> onModelCreating;

		public EndToEndTestDbContext(Action<ModelBuilder> onModelCreating = default)
		{
			this.onModelCreating = onModelCreating;
		}

		protected override BusinessLayerDbContextSettings CreateDbContextSettings()
		{
			var settings = base.CreateDbContextSettings();

			settings.UseCharColumnTypeForCharPropertyConvention = false;
			settings.UseCollectionExtendedPropertiesConvention = false;
			settings.UseDefaultValueAttributeConvention = false;
			settings.UseDefaultValueSqlAttributeConvention = false;
			settings.UseForeignKeysColumnNamesConvention = false;
			settings.UseForeignKeysIndexConvention = false;
			settings.UseLanguageUiCultureIndexConvention = false;
			settings.LocalizationTableIndexConvention = false;
			settings.UseLocalizationTablesParentEntitiesConvention = false;
			settings.UseNamespaceExtendedPropertyConvention = false;
			settings.UsePrefixedTablePrimaryKeysConvention = false;
			settings.UseXmlCommentsForDescriptionPropertyConvention = false;

			return settings;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseFrameworkConventions(frameworkConventions => frameworkConventions
				.UseCacheAttributeToAnnotationConvention(false)
				.UseCascadeDeleteToRestrictConvention(false)
				.UseDataTypeAttributeConvention(false)
				.UseManyToManyEntityKeyDiscoveryConvention(false)
				.UseStringPropertiesDefaultValueConvention(false)
			);

			// stub out Model Extender types, so all extenders in test assembly don't interfere with tests.
			// Tests should setup their own types when necessary.
			optionsBuilder.SetModelExtenderTypes(Enumerable.Empty<TypeInfo>());
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);
			onModelCreating?.Invoke(modelBuilder);
		}

		public IReadOnlyList<MigrationOperation> Diff(DbContext target)
		{
			var differ = this.GetService<IMigrationsModelDiffer>();
			return differ.GetDifferences(this.GetService<IDesignTimeModel>().Model.GetRelationalModel(), target.GetService<IDesignTimeModel>().Model.GetRelationalModel());
		}

		public IReadOnlyList<MigrationCommand> Migrate(DbContext target)
		{
			var diff = Diff(target);
			var generator = this.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(diff, this.Model);
		}
	}
}