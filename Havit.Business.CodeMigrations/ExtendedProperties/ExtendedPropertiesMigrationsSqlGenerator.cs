using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
	{
		private const string DefaultSchemaName = "dbo";

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
				AddExtendedPropertyLevel1(name, value, GetSchema(operation.Schema, model), operation.Name, builder);
			}
			foreach (var column in operation.Columns)
			{
				foreach (var annotation in column.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
				{
					var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
					var value = (string)annotation.Value;
					AddExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), column.Table, column.Name, builder);
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
				AddExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
		}

		protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var oldAnnotations = operation.OldTable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				DropExtendedPropertyLevel1(name, GetSchema(operation.Schema, model), operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				UpdateExtendedPropertyLevel1(name, value, GetSchema(operation.Schema, model), operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(name, value, GetSchema(operation.Schema, model), operation.Name, builder);
			}
		}

		protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var oldAnnotations = operation.OldColumn.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				DropExtendedPropertyLevel2(name, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				UpdateExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
		}

		protected override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			var oldAnnotations = operation.OldDatabase.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotations = operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation).ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				DropExtendedPropertyLevelNothing(name, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				UpdateExtendedPropertyLevelNothing(name, value, builder);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevelNothing(name, value, builder);
			}
		}

		protected override void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var name = ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation);
				var value = (string)annotation.Value;
				AddExtendedPropertyLevelNothing(name, value, builder);
			}
		}

		private void AddExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append("")
				.EndCommand();
		}

		private void UpdateExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append("")
				.EndCommand();
		}

		private void DropExtendedPropertyLevel1(string name, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append("")
				.EndCommand();
		}

		private void AddExtendedPropertyLevel2(string name, string value, string schemaName, string tableName, string columnName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(columnName))
				.Append("")
				.EndCommand();
		}

		private void UpdateExtendedPropertyLevel2(string name, string value, string schemaName, string tableName, string columnName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(columnName))
				.Append("")
				.EndCommand();
		}

		private void DropExtendedPropertyLevel2(string name, string schemaName, string tableName, string columnName, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(columnName))
				.Append("")
				.EndCommand();
		}

		private void AddExtendedPropertyLevelNothing(string name, string value, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append("")
				.EndCommand();
		}

		private void UpdateExtendedPropertyLevelNothing(string name, string value, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append("")
				.EndCommand();
		}

		private void DropExtendedPropertyLevelNothing(string name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append("")
				.EndCommand();
		}

		private string GenerateSqlLiteral(string s) => Dependencies.TypeMappingSource.GetMapping(typeof(string)).GenerateSqlLiteral(s);

		private static string GetSchema(string operationSchema, IModel model) => operationSchema ?? (string)model.Relational().DefaultSchema ?? DefaultSchemaName;
	}
}
