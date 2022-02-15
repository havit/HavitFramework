using System.ComponentModel.DataAnnotations.Schema;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties
{
	public class ExtendedPropertiesEndToEnd_EncodingTests
	{
		[TestClass]
		public class AddingPropertyToColumn
		{
			private const string ExtendedPropertyName = "MyProp";

			[Table("Table")]
			private class SourceEntity
			{
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				public int Id { get; set; }

				[TestExtendedProperties(ExtendedPropertyName, @"Hello
World")]
				public string Property { get; set; }
			}

			[TestMethod]
			public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumn_StringWithNewLines()
			{
				var expectedVariableName = $"dbo_Table_{nameof(TargetEntity.Property)}_{ExtendedPropertyName}_value";

				var source = new EndToEndTestDbContext<SourceEntity>();
				var target = new EndToEndTestDbContext<TargetEntity>();
				var migrations = source.Migrate(target);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					$@"DECLARE @{expectedVariableName} NVARCHAR(4000) = CONCAT(CAST(N'Hello' AS nvarchar(max)), nchar(13), nchar(10), N'World');
EXEC sys.sp_addextendedproperty @name=N'{ExtendedPropertyName}', @value=@{expectedVariableName}, @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'{nameof(TargetEntity.Property)}';
", migrations[1].CommandText);
			}
		}

		[TestClass]
		public class UpdatingExistingStringPropertyOnColumn
		{
			private const string ExtendedPropertyName = "MyProp";

			[Table("Table")]
			private class SourceEntity
			{
				public int Id { get; set; }

				public string Property { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				public int Id { get; set; }

				public string Property { get; set; }
			}

			[TestMethod]
			public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyOnColumn_NewValueIsStringWithNewLines()
			{
				var expectedVariableName = $"dbo_Table_{nameof(TargetEntity.Property)}_{ExtendedPropertyName}_value";

				var source = new EndToEndTestDbContext<SourceEntity>(x => x
					.Entity<SourceEntity>()
					.Property(e => e.Property)
					.AddExtendedProperty(ExtendedPropertyName, "Single line"));
				var target = new EndToEndTestDbContext<TargetEntity>(x => x
					.Entity<TargetEntity>()
					.Property(e => e.Property)
					.AddExtendedProperty(ExtendedPropertyName, @"Hello
World"));
				var migrations = source.Migrate(target);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					$@"DECLARE @{expectedVariableName} NVARCHAR(4000) = CONCAT(CAST(N'Hello' AS nvarchar(max)), nchar(13), nchar(10), N'World');
EXEC sys.sp_updateextendedproperty @name=N'{ExtendedPropertyName}', @value=@{expectedVariableName}, @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'{nameof(TargetEntity.Property)}';
", migrations[1].CommandText);
			}
		}

		[TestClass]
		public class AddingPropertyToTable
		{
			private const string ExtendedPropertyName = "MyProp";

			[Table("Table")]
			private class SourceEntity
			{
				public int Id { get; set; }
			}

			[Table("Table")]
			[TestExtendedProperties(ExtendedPropertyName, @"Hello
World")]
			private class TargetEntity
			{
				public int Id { get; set; }
			}

			[TestMethod]
			public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumn_StringWithNewLines()
			{
				var expectedVariableName = $"dbo_Table_{ExtendedPropertyName}_value";

				var source = new EndToEndTestDbContext<SourceEntity>();
				var target = new EndToEndTestDbContext<TargetEntity>();
				var migrations = source.Migrate(target);
				
				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					$@"DECLARE @{expectedVariableName} NVARCHAR(4000) = CONCAT(CAST(N'Hello' AS nvarchar(max)), nchar(13), nchar(10), N'World');
EXEC sys.sp_addextendedproperty @name=N'{ExtendedPropertyName}', @value=@{expectedVariableName}, @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
", migrations[0].CommandText);
			}
		}

		[TestClass]
		public class UpdatingPropertyOnTable
		{
			private const string ExtendedPropertyName = "MyProp";

			[Table("Table")]
			[TestExtendedProperties(ExtendedPropertyName, "Some value")]
			private class SourceEntity
			{
				public int Id { get; set; }
			}

			[Table("Table")]
			[TestExtendedProperties(ExtendedPropertyName, @"Hello
World")]
			private class TargetEntity
			{
				public int Id { get; set; }
			}

			[TestMethod]
			public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumn_StringWithNewLines()
			{
				var expectedVariableName = $"dbo_Table_{ExtendedPropertyName}_value";

				var source = new EndToEndTestDbContext<SourceEntity>();
				var target = new EndToEndTestDbContext<TargetEntity>();
				var migrations = source.Migrate(target);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					$@"DECLARE @{expectedVariableName} NVARCHAR(4000) = CONCAT(CAST(N'Hello' AS nvarchar(max)), nchar(13), nchar(10), N'World');
EXEC sys.sp_updateextendedproperty @name=N'{ExtendedPropertyName}', @value=@{expectedVariableName}, @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
", migrations[0].CommandText);
			}
		}
	}
}