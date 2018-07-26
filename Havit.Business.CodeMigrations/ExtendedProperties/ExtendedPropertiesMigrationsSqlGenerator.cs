using System;
using System.Collections.Generic;
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

			AlterHelper(operation.OldTable.GetAnnotations(), operation.GetAnnotations(),
				(a, name) =>
				{
					DropExtendedPropertyLevel1(name, GetSchema(operation.Schema, model), operation.Name, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					UpdateExtendedPropertyLevel1(name, value, GetSchema(operation.Schema, model), operation.Name, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					AddExtendedPropertyLevel1(name, value, GetSchema(operation.Schema, model), operation.Name, builder);
				});
		}

		protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			AlterHelper(operation.OldColumn.GetAnnotations(), operation.GetAnnotations(),
				(a, name) =>
				{
					DropExtendedPropertyLevel2(name, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					UpdateExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					AddExtendedPropertyLevel2(name, value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				});
		}

		protected override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			base.Generate(operation, model, builder);

			AlterHelper(operation.OldDatabase.GetAnnotations(), operation.GetAnnotations(),
				(a, name) =>
				{
					DropExtendedPropertyLevelNothing(name, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					UpdateExtendedPropertyLevelNothing(name, value, builder);
				},
				(a, name) =>
				{
					var value = (string)a.Value;
					AddExtendedPropertyLevelNothing(name, value, builder);
				});
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

		private static void AlterHelper(IEnumerable<IAnnotation> oldAnnotations, IEnumerable<IAnnotation> newAnnotations,
			Action<IAnnotation, string> dropAction, Action<IAnnotation, string> updateAction, Action<IAnnotation, string> addAction)
		{
			oldAnnotations = oldAnnotations.Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
			newAnnotations = newAnnotations.Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
			var oldAnnotationsLookup = oldAnnotations.ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotationsLookup = newAnnotations.ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotationsLookup.ContainsKey(x.Name)))
			{
				dropAction(annotation, ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation));
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotationsLookup.ContainsKey(x.Name)))
			{
				updateAction(annotation, ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation));
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotationsLookup.ContainsKey(x.Name)))
			{
				addAction(annotation, ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation));
			}
		}
	}
}
