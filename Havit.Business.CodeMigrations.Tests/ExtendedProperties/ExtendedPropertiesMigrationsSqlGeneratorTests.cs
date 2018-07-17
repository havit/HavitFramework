using System.Collections.Generic;
using System.Data.SqlClient;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
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
				Name = "TableName"
			};
			var columnOperation = new AddColumnOperation()
			{
				Table = tableOperation.Name,
				Name = "ColumnName",
				ClrType = typeof(int)
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
				Name = "TableName"
			};
			var columnOperation = new AddColumnOperation()
			{
				Table = tableOperation.Name,
				Name = "ColumnName",
				ClrType = typeof(int)
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

		private class TestDbContext : BusinessLayerDbContext
		{
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);
				optionsBuilder.UseSqlServer(new SqlConnection("Database=DummyDatabase"));
			}
		}

		private class TestExtendedPropertyAttribute : ExtendedPropertyAttribute
		{
			public override string Name { get; }
			public override string Value { get; }

			public TestExtendedPropertyAttribute(string name, string value)
			{
				Name = name;
				Value = value;
			}
		}

		private IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations)
		{
			using (var db = new TestDbContext())
			{
				var generator = db.GetService<IMigrationsSqlGenerator>();
				return generator.Generate(operations);
			}
		}
	}
}
