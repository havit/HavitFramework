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

			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(name, value, operation.Name, builder);
			}
			foreach (var column in operation.Columns)
			{
				foreach (var annotation in column.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
				{
					var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
					var value = (string)annotation.Value;
					AddExtendedPropertyLevel2(name, value, column.Table, column.Name, builder);
				}
			}
		}

		protected override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel2(name, value, operation.Table, operation.Name, builder);
			}
		}

		protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var oldAnnotations = operation.OldTable.GetAnnotations().ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				DropExtendedPropertyLevel1(name, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				UpdateExtendedPropertyLevel1(name, value, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(name, value, operation.Name, builder);
			}
		}

		protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var oldAnnotations = operation.OldColumn.GetAnnotations().ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				DropExtendedPropertyLevel2(name, operation.Table, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				UpdateExtendedPropertyLevel2(name, value, operation.Table, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel2(name, value, operation.Table, operation.Name, builder);
			}
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

		private void UpdateExtendedPropertyLevel1(string name, string value, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append("")
				.EndCommand();
		}

		private void DropExtendedPropertyLevel1(string name, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
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

		private void UpdateExtendedPropertyLevel2(string name, string value, string level1Name, string level2Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
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

		private void DropExtendedPropertyLevel2(string name, string level1Name, string level2Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
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
