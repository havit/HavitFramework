using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

			Settings.UseCacheAttributeToAnnotationConvention = false;
			Settings.UseCascadeDeleteToRestrictConvention = false;
			Settings.UseCharColumnTypeForCharPropertyConvention = false;
			Settings.UseCollectionExtendedPropertiesConvention = false;
			Settings.UseDataTypeAttributeConvention = false;
			Settings.UseDefaultValueAttributeConvention = false;
			Settings.UseDefaultValueSqlAttributeConvention = false;
			Settings.UseForeignKeysColumnNamesConvention = false;
			Settings.UseIndexForForeignKeysConvention = false;
			Settings.UseIndexForLanguageUiCulturePropertyConvention = false;
			Settings.UseIndexForLocalizationTableConvention = false;
			// TODO EF Core 3.0: Vyhodit
			//Settings.UseIndexNamingConvention = false;
			Settings.UseLocalizationTablesParentEntitiesConvention = false;
			Settings.UseManyToManyEntityKeyDiscoveryConvention = false;
			Settings.UseNamespaceExtendedPropertyConvention = false;
			Settings.UsePrefixedTablePrimaryKeysConvention = false;
			Settings.UseStringPropertiesDefaultValueConvention = false;
			Settings.UseXmlCommentsForDescriptionPropertyConvention = false;
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);
			onModelCreating?.Invoke(modelBuilder);
		}

		public IReadOnlyList<MigrationOperation> Diff(DbContext target)
		{
			var differ = this.GetService<IMigrationsModelDiffer>();
			return differ.GetDifferences(Model, target.Model);
		}

		public IReadOnlyList<MigrationCommand> Migrate(DbContext target)
		{
			var diff = Diff(target);
			var generator = this.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(diff, this.Model);
		}
	}
}