using System.Collections.Generic;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	[TestClass]
	public class ExtendedPropertiesMigrationsSqlGeneratorTests
	{
		[TestMethod]
		public void CreateTable_TableWithExtendedProperty()
		{
			var tableOperation = new CreateTableOperation()
			{
				Name = "TableName",
			};
			var columnOperation = new AddColumnOperation()
			{
				Table = tableOperation.Name,
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			tableOperation.Columns.Add(columnOperation);
			var attr = new TestExtendedPropertyAttribute("OnTable", "TableValue");
			tableOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attr), attr.Value);
			var migrations = Generate(new[] { tableOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'OnTable', @value=N'TableValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName'",
				migrations[1].CommandText);
		}

		[TestMethod]
		public void CreateTable_ColumnWithExtendedProperty()
		{
			var tableOperation = new CreateTableOperation()
			{
				Name = "TableName",
			};
			var columnOperation = new AddColumnOperation()
			{
				Table = tableOperation.Name,
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			tableOperation.Columns.Add(columnOperation);
			var attr = new TestExtendedPropertyAttribute("OnColumn", "ColumnValue");
			columnOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attr), attr.Value);
			var migrations = Generate(new[] { tableOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'OnColumn', @value=N'ColumnValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName', @level2type=N'COLUMN', @level2name=N'ColumnName'",
				migrations[1].CommandText);
		}

		[TestMethod]
		public void AddColumn_ColumnWithExtendedProperty()
		{
			var columnOperation = new AddColumnOperation()
			{
				Table = "TableName",
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			var attr = new TestExtendedPropertyAttribute("OnColumn", "ColumnValue");
			columnOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attr), attr.Value);
			var migrations = Generate(new[] { columnOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'OnColumn', @value=N'ColumnValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName', @level2type=N'COLUMN', @level2name=N'ColumnName'",
				migrations[1].CommandText);
		}

		[TestMethod]
		public void AlterTable_TableWithUpdatedExtendedProperty()
		{
			var tableOperation = new AlterTableOperation()
			{
				Name = "TableName",
			};
			var attrOld = new TestExtendedPropertyAttribute("OnTable", "OldValue");
			var attrNew = new TestExtendedPropertyAttribute("OnTable", "NewValue");
			tableOperation.OldTable.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrOld), attrOld.Value);
			tableOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrNew), attrNew.Value);
			var migrations = Generate(new[] { tableOperation });

			Assert.AreEqual(1, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_updateextendedproperty @name=N'OnTable', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName'",
				migrations[0].CommandText);
		}

		[TestMethod]
		public void AlterTable_TableWithRemovedExtendedProperty()
		{
			var tableOperation = new AlterTableOperation()
			{
				Name = "TableName",
			};
			var attrOld = new TestExtendedPropertyAttribute("OnTable", "OldValue");
			tableOperation.OldTable.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrOld), attrOld.Value);
			var migrations = Generate(new[] { tableOperation });

			Assert.AreEqual(1, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_dropextendedproperty @name=N'OnTable', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName'",
				migrations[0].CommandText);
		}

		[TestMethod]
		public void AlterTable_TableWithAddedExtendedProperty()
		{
			var tableOperation = new AlterTableOperation()
			{
				Name = "TableName",
			};
			var attrNew = new TestExtendedPropertyAttribute("OnTable", "NewValue");
			tableOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrNew), attrNew.Value);
			var migrations = Generate(new[] { tableOperation });

			Assert.AreEqual(1, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'OnTable', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName'",
				migrations[0].CommandText);
		}

		[TestMethod]
		public void AlterColumn_ColumnWithUpdatedExtendedProperty()
		{
			var columnOperation = new AlterColumnOperation()
			{
				Table = "TableName",
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			columnOperation.OldColumn.ClrType = columnOperation.ClrType;
			var attrOld = new TestExtendedPropertyAttribute("OnTable", "OldValue");
			var attrNew = new TestExtendedPropertyAttribute("OnTable", "NewValue");
			columnOperation.OldColumn.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrOld), attrOld.Value);
			columnOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrNew), attrNew.Value);
			var migrations = Generate(new[] { columnOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_updateextendedproperty @name=N'OnTable', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName', @level2type=N'COLUMN', @level2name=N'ColumnName'",
				migrations[1].CommandText);
		}

		[TestMethod]
		public void AlterColumn_ColumnWithRemovedExtendedProperty()
		{
			var columnOperation = new AlterColumnOperation()
			{
				Table = "TableName",
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			columnOperation.OldColumn.ClrType = columnOperation.ClrType;
			var attrOld = new TestExtendedPropertyAttribute("OnTable", "OldValue");
			columnOperation.OldColumn.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrOld), attrOld.Value);
			var migrations = Generate(new[] { columnOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_dropextendedproperty @name=N'OnTable', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName', @level2type=N'COLUMN', @level2name=N'ColumnName'",
				migrations[1].CommandText);
		}

		[TestMethod]
		public void AlterColumn_ColumnWithAddedExtendedProperty()
		{
			var columnOperation = new AlterColumnOperation()
			{
				Table = "TableName",
				Name = "ColumnName",
				ClrType = typeof(int),
			};
			columnOperation.OldColumn.ClrType = columnOperation.ClrType;
			var attrNew = new TestExtendedPropertyAttribute("OnTable", "NewValue");
			columnOperation.AddAnnotation(ExtendedPropertiesAnnotationsHelper.BuildAnnotationName(attrNew), attrNew.Value);
			var migrations = Generate(new[] { columnOperation });

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'OnTable', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableName', @level2type=N'COLUMN', @level2name=N'ColumnName'",
				migrations[1].CommandText);
		}

		private IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations)
		{
			using (var db = new TestDbContext())
			{
				var generator = db.GetService<IMigrationsSqlGenerator>();
				return generator.Generate(operations, db.Model);
			}
		}
	}
}
