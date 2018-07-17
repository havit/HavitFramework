using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
	{
		public ExtendedPropertiesMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IMigrationsAnnotationProvider migrationsAnnotations)
			: base(dependencies, migrationsAnnotations)
		{ }

		protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var tableName = operation.Name;
			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.AnnotationsFilter))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(name, value, tableName, builder);
			}
			foreach (var column in operation.Columns)
			{
				var columnName = column.Name;
				foreach (var annotation in column.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.AnnotationsFilter))
				{
					var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
					var value = (string)annotation.Value;
					AddExtendedPropertyLevel2(name, value, tableName, columnName, builder);
				}
			}
		}

		protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);
		}

		protected override void Generate(DropTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);
		}

		protected override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);
		}

		protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);
		}

		protected override void Generate(DropColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);
		}

		private void AddExtendedPropertyLevel1(string name, string value, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append("")
				.EndCommand();
		}

		private void AddExtendedPropertyLevel2(string name, string value, string level1Name, string level2Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(level2Name))
				.Append("")
				.EndCommand();
		}

		private string GenerateSqlLiteral(string s) => Dependencies.TypeMappingSource.GetMapping(typeof(string)).GenerateSqlLiteral(s);
	}
}
