using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties
{
	internal class ExtendedPropertiesMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
	{
	    private const string DefaultSchemaName = "dbo";
	    private const string TableLevel1Type = "TABLE";

	    private readonly IRelationalTypeMappingSource typeMappingSource;
        private readonly ISqlGenerationHelper sqlGenerationHelper;

        public ExtendedPropertiesMigrationOperationSqlGenerator(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlGenerationHelper sqlGenerationHelper)
        {
            this.typeMappingSource = typeMappingSource;
            this.sqlGenerationHelper = sqlGenerationHelper;
        }

        public override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel1(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), operation.Name, builder);
			}
			foreach (var column in operation.Columns)
			{
				foreach (var annotation in column.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
				{
					var value = (string)annotation.Value;
					AddExtendedPropertyLevel2(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), column.Table, column.Name, builder);
				}
			}
		}

		public override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				AddExtendedPropertyLevel2(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation), value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder);
			}
		}

		public override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldTable.GetAnnotations(), operation.GetAnnotations(),
				dropAction: a => DropExtendedPropertyLevel1(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), GetSchema(operation.Schema, model), operation.Name, builder),
				updateAction: a => UpdateExtendedPropertyLevel1(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), (string)a.Value, GetSchema(operation.Schema, model), operation.Name, builder),
				addAction: a => AddExtendedPropertyLevel1(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), (string)a.Value, GetSchema(operation.Schema, model), operation.Name, builder));
		}

		public override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldColumn.GetAnnotations(), operation.GetAnnotations(),
				a => DropExtendedPropertyLevel2(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), GetSchema(operation.Schema, model), operation.Table, operation.Name, builder),
				a => UpdateExtendedPropertyLevel2(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), (string)a.Value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder),
				a => AddExtendedPropertyLevel2(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), (string)a.Value, GetSchema(operation.Schema, model), operation.Table, operation.Name, builder));
		}

		public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			AlterHelper(operation.OldDatabase.GetAnnotations(), operation.GetAnnotations(),
				a =>
				{
					if (ExtendedPropertiesAnnotationsHelper.TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						DropExtendedPropertyLevel1WithType(name, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						DropExtendedPropertyLevelNothing(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), builder);
					}
				},
				a =>
				{
					var value = (string)a.Value;
					if (ExtendedPropertiesAnnotationsHelper.TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						UpdateExtendedPropertyLevel1WithType(name, value, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						UpdateExtendedPropertyLevelNothing(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), value, builder);
					}
				},
				a =>
				{
					var value = (string)a.Value;
					if (ExtendedPropertiesAnnotationsHelper.TryParseExtraDatabaseObjectAnnotationName(a, out var schema, out var level1Type, out var level1Name, out var name))
					{
						AddExtendedPropertyLevel1WithType(name, value, GetSchema(schema, model), level1Type, level1Name, builder);
					}
					else
					{
						AddExtendedPropertyLevelNothing(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(a), value, builder);
					}
				});
		}

		public override void Generate(SqlServerCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
		{
			foreach (var annotation in operation.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation))
			{
				var value = (string)annotation.Value;
				if (ExtendedPropertiesAnnotationsHelper.TryParseExtraDatabaseObjectAnnotationName(annotation, out var schema, out var level1Type, out var level1Name, out var name))
				{
					AddExtendedPropertyLevel1WithType(name, value, schema, level1Type, level1Name, builder);
				}
				else
				{
					AddExtendedPropertyLevelNothing(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName(annotation), value, builder);
				}
			}
		}

		private void AddExtendedPropertyLevel1WithType(string name, string value, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
			string valueVariable = null;
			string propertyValue = GenerateSqlLiteral(value);
			if (Regex.IsMatch(propertyValue, @"^(concat\(|cast\()+", RegexOptions.IgnoreCase | RegexOptions.Compiled))
			{
				valueVariable = $"@{schemaName}_{level1Name}_{name}_value";

				builder
					.AppendLine($"DECLARE {valueVariable} NVARCHAR(4000) = {propertyValue};");
			}

			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=");

			if (!string.IsNullOrEmpty(valueVariable))
			{
				builder.Append(valueVariable);
			}
			else
			{
				builder.Append(propertyValue);
			}

			builder
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'")
				.Append(level1Type)
				.Append("', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.AppendLine(";")
				.EndCommand();
		}
		private void AddExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			AddExtendedPropertyLevel1WithType(name, value, schemaName, TableLevel1Type, tableName, builder);
		}

		private void UpdateExtendedPropertyLevel1WithType(string name, string value, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
			string valueVariable = null;
			string propertyValue = GenerateSqlLiteral(value);
			if (Regex.IsMatch(propertyValue, @"^(concat\(|cast\()+", RegexOptions.IgnoreCase | RegexOptions.Compiled))
			{
				valueVariable = $"@{schemaName}_{level1Name}_{name}_value";

				builder
					.AppendLine($"DECLARE {valueVariable} NVARCHAR(4000) = {propertyValue};");
			}

			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=");

			if (!string.IsNullOrEmpty(valueVariable))
			{
				builder.Append(valueVariable);
			}
			else
			{
				builder.Append(propertyValue);
			}

			builder
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'")
				.Append(level1Type)
				.Append("', @level1name=")
				.Append(GenerateSqlLiteral(level1Name))
				.AppendLine(";")
				.EndCommand();
		}
		private void UpdateExtendedPropertyLevel1(string name, string value, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			UpdateExtendedPropertyLevel1WithType(name, value, schemaName, TableLevel1Type, tableName, builder);
		}

		private void DropExtendedPropertyLevel1WithType(string name, string schemaName, string level1Type, string level1Name, MigrationCommandListBuilder builder)
		{
            builder
                .Append("IF OBJECT_ID(")
                .Append(GenerateSqlLiteral(sqlGenerationHelper.DelimitIdentifier(level1Name, schemaName)))
                .AppendLine(") IS NOT NULL")
                .AppendLine("BEGIN");

            using (builder.Indent())
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
                    .AppendLine(";");
            }

            builder.AppendLine("END")
                .EndCommand();
        }
		private void DropExtendedPropertyLevel1(string name, string schemaName, string tableName, MigrationCommandListBuilder builder)
		{
			DropExtendedPropertyLevel1WithType(name, schemaName, TableLevel1Type, tableName, builder);
		}

		private void AddExtendedPropertyLevel2(string name, string value, string schemaName, string tableName, string columnName, MigrationCommandListBuilder builder)
		{
			string valueVariable = null;
			string propertyValue = GenerateSqlLiteral(value);
			if (Regex.IsMatch(propertyValue, @"^(concat\(|cast\()+", RegexOptions.IgnoreCase | RegexOptions.Compiled))
			{
				valueVariable = $"@{schemaName}_{tableName}_{columnName}_{name}_value";

				builder
					.AppendLine($"DECLARE {valueVariable} NVARCHAR(4000) = {propertyValue};");
			}

			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=");

			if (!string.IsNullOrEmpty(valueVariable))
			{
				builder.Append(valueVariable);
			}
			else
			{
				builder.Append(propertyValue);
			}

			builder
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(columnName))
				.AppendLine(";")
				.EndCommand();
		}

		private void UpdateExtendedPropertyLevel2(string name, string value, string schemaName, string tableName, string columnName, MigrationCommandListBuilder builder)
		{
			string valueVariable = null;
			string propertyValue = GenerateSqlLiteral(value);
			if (Regex.IsMatch(propertyValue, @"^(concat\(|cast\()+", RegexOptions.IgnoreCase | RegexOptions.Compiled))
			{
				valueVariable = $"@{schemaName}_{tableName}_{columnName}_{name}_value";

				builder
					.AppendLine($"DECLARE {valueVariable} NVARCHAR(4000) = {propertyValue};");
			}

			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=");

			if (!string.IsNullOrEmpty(valueVariable))
			{
				builder.Append(valueVariable);
			}
			else
			{
				builder.Append(propertyValue);
			}

			builder
				.Append(", @level0type=N'SCHEMA', @level0name=")
				.Append(GenerateSqlLiteral(schemaName))
				.Append(", @level1type=N'TABLE', @level1name=")
				.Append(GenerateSqlLiteral(tableName))
				.Append(", @level2type=N'COLUMN', @level2name=")
				.Append(GenerateSqlLiteral(columnName))
				.AppendLine(";")
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
				.AppendLine(";")
				.EndCommand();
		}

		private void AddExtendedPropertyLevelNothing(string name, string value, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_addextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.AppendLine(";")
				.EndCommand();
		}

		private void UpdateExtendedPropertyLevelNothing(string name, string value, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_updateextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.Append(", @value=")
				.Append(GenerateSqlLiteral(value))
				.AppendLine(";")
				.EndCommand();
		}

		private void DropExtendedPropertyLevelNothing(string name, MigrationCommandListBuilder builder)
		{
			builder
				.Append("EXEC sys.sp_dropextendedproperty @name=")
				.Append(GenerateSqlLiteral(name))
				.AppendLine(";")
				.EndCommand();
		}

		private string GenerateSqlLiteral(string s) => typeMappingSource.GetMapping(typeof(string)).GenerateSqlLiteral(s);

		private static string GetSchema(string operationSchema, IModel model) => operationSchema ?? model.GetDefaultSchema() ?? DefaultSchemaName;

		private static void AlterHelper(IEnumerable<IAnnotation> oldAnnotations, IEnumerable<IAnnotation> newAnnotations,
			Action<IAnnotation> dropAction, Action<IAnnotation> updateAction, Action<IAnnotation> addAction)
		{
			oldAnnotations = oldAnnotations.Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
			newAnnotations = newAnnotations.Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
			var oldAnnotationsLookup = oldAnnotations.ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
			var newAnnotationsLookup = newAnnotations.ToDictionary(x => x.Name, ExtendedPropertiesAnnotationsHelper.Comparer);
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
