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

			Settings.UseCacheAttributeToAnnotationConvention = false;
			Settings.UseCascadeDeleteToRestrictConvention = false;
			Settings.UseCharColumnTypeForCharPropertyConvention = false;
			Settings.UseCollectionExtendedPropertiesConvention = false;
			Settings.UseDataTypeAttributeConvention = false;
			Settings.UseDefaultValueAttributeConvention = false;
			Settings.UseDefaultValueSqlAttributeConvention = false;
			Settings.UseForeignKeysColumnNamesConvention = false;
			Settings.UseForeignKeysIndexConvention = false;
			Settings.UseLanguageUiCultureIndexConvention = false;
			Settings.LocalizationTableIndexConvention = false;
			Settings.UseLocalizationTablesParentEntitiesConvention = false;
			Settings.UseManyToManyEntityKeyDiscoveryConvention = false;
			Settings.UseNamespaceExtendedPropertyConvention = false;
			Settings.UsePrefixedTablePrimaryKeysConvention = false;
			Settings.UseStringPropertiesDefaultValueConvention = false;
			Settings.UseXmlCommentsForDescriptionPropertyConvention = false;
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

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