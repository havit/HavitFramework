using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using static Havit.Business.CodeMigrations.ExtendedProperties.ExtendedPropertiesAnnotationsHelper;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
	{
	    private const string DefaultSchemaName = "dbo";
	    private const string TableLevel1Type = "TABLE";

	    private readonly IRelationalTypeMappingSource typeMappingSource;

	    public ExtendedPropertiesMigrationOperationSqlGenerator(IRelationalTypeMappingSource typeMappingSource)
		{
		    this.typeMappingSource = typeMappingSource;
		}

        public override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), operation.Name, builder);
			}
			foreach (var column in operation.Columns)
			{
				foreach (var annotation in column.GetAnnotations().Where(IsExtendedPropertyAnnotation))
				{
					var value = (string)annotation.Value;
					AddExtendedPropertyLevel2(ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), column.Table, column.Name, builder);
				}
			}
		}

		public override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel2(ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
		}

		public override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldTable.GetAnnotations(), operation.GetAnnotations(),
				a =>
				{
					DropExtendedPropertyLevel1(ParseAnnotationName(a), GetSchema(operation.Schema, model), operation.Name, builder);
				},
				a =>
				{
					var value = (string)a.Value;
					UpdateExtendedPropertyLevel1(ParseAnnotationName(a), value, GetSchema(operation.Schema, model), operation.Name, builder);
				},
				a =>
				{
					var value = (string)a.Value;
					AddExtendedPropertyLevel1(ParseAnnotationName(a), value, GetSchema(operation.Schema, model), operation.Name, builder);
				});
		}

		public override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldColumn.GetAnnotations(), operation.GetAnnotations(),
				a =>
				{
					DropExtendedPropertyLevel2(ParseAnnotationName(a), GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				},
				a =>
				{
					var value = (string)a.Value;
					UpdateExtendedPropertyLevel2(ParseAnnotationName(a), value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				},
				a =>
				{
					var value = (string)a.Value;
					AddExtendedPropertyLevel2(ParseAnnotationName(a), value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
				});
		}

		public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldDatabase.GetAnnotations(), operation.GetAnnotations(),
				a =>
				{
					if (TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						DropExtendedPropertyLevel1WithType(name, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						DropExtendedPropertyLevelNothing(ParseAnnotationName(a), builder);
					}
				},
				a =>
				{
					var value = (string)a.Value;
					if (TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						UpdateExtendedPropertyLevel1WithType(name, value, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						UpdateExtendedPropertyLevelNothing(ParseAnnotationName(a), value, builder);
					}
				},
				a =>
				{
					var value = (string)a.Value;
					if (TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						AddExtendedPropertyLevel1WithType(name, value, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						AddExtendedPropertyLevelNothing(ParseAnnotationName(a), value, builder);
					}
				});
		}

		public override void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				if (TryParseExtraDatabaseObjectAnnotationName(annotation, out var schema, out var level1Type, out var level1Name, out var name))
				{
					AddExtendedPropertyLevel1WithType(name, value, schema, level1Type, level1Name, builder);
				}
				else
				{
					AddExtendedPropertyLevelNothing(ParseAnnotationName(annotation), value, builder);
				}
			}
		}

		private void AddExtendedPropertyLevel1WithType(string name, string value, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'")
				.Append(level1Type)
				.Append("', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append("")
				.EndCommand();
		}
		private void AddExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			AddExtendedPropertyLevel1WithType(name, value, schemaName, TableLevel1Type, tableName, builder);
		}

		private void UpdateExtendedPropertyLevel1WithType(string name, string value, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'")
				.Append(level1Type)
				.Append("', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append("")
				.EndCommand();
		}
		private void UpdateExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			UpdateExtendedPropertyLevel1WithType(name, value, schemaName, TableLevel1Type, tableName, builder);
		}

		private void DropExtendedPropertyLevel1WithType(string name, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'")
				.Append(level1Type)
				.Append("', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.Append("")
				.EndCommand();
		}
		private void DropExtendedPropertyLevel1(string name, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			DropExtendedPropertyLevel1WithType(name, schemaName, TableLevel1Type, tableName, builder);
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

		private string GenerateSqlLiteral(string s) => typeMappingSource.GetMapping(typeof(string)).GenerateSqlLiteral(s);

		private static string GetSchema(string operationSchema, IModel model) => operationSchema ?? (string)model.Relational().DefaultSchema ?? DefaultSchemaName;

		private static void AlterHelper(IEnumerable<IAnnotation> oldAnnotations, IEnumerable<IAnnotation> newAnnotations,
			Action<IAnnotation> dropAction, Action<IAnnotation> updateAction, Action<IAnnotation> addAction)
		{
			oldAnnotations = oldAnnotations.Where(IsExtendedPropertyAnnotation);
			newAnnotations = newAnnotations.Where(IsExtendedPropertyAnnotation);
			var oldAnnotationsLookup = oldAnnotations.ToDictionary(x => x.Name, Comparer);
			var newAnnotationsLookup = newAnnotations.ToDictionary(x => x.Name, Comparer);
			foreach (var annotation in oldAnnotations.Where(x => !newAnnotationsLookup.ContainsKey(x.Name)))
			{
				dropAction(annotation);
			}
			foreach (var annotation in newAnnotations.Where(x => oldAnnotationsLookup.ContainsKey(x.Name)))
			{
				updateAction(annotation);
			}
			foreach (var annotation in newAnnotations.Where(x => !oldAnnotationsLookup.ContainsKey(x.Name)))
			{
				addAction(annotation);
			}
		}
	}
}
