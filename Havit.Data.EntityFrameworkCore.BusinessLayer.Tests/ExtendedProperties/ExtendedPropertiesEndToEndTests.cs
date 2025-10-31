using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties;

public class ExtendedPropertiesEndToEndTests
{
	[TestClass]
	public class AddingPropertyToTable
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[0].CommandText);
		}
	}
	[TestClass]
	public class AddingPropertyToTableInNonDefaultSchema
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var target = new EndToEndTestDbContext<TargetEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'custom_schema', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToTableUsingExtension
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToTableUsingExtension()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>(builder =>
			{
				builder.Entity<TargetEntity>().AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Value" } });
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyFromTable
	{
		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyFromTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[Table]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyFromTableInNonDefaultSchema
	{
		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyFromTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var target = new EndToEndTestDbContext<TargetEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[custom_schema].[Table]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'custom_schema', @level1type=N'TABLE', @level1name=N'Table';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class ChangingPropertyOnTable
	{
		[TestExtendedProperties("Jiri", "OldValue")]
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[TestExtendedProperties("Jiri", "NewValue")]
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingPropertyOnTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class ChangingPropertyOnTableInNonDefaultSchema
	{
		[TestExtendedProperties("Jiri", "OldValue")]
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[TestExtendedProperties("Jiri", "NewValue")]
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingPropertyOnTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var target = new EndToEndTestDbContext<TargetEntity>(builder => builder.HasDefaultSchema("custom_schema"));
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'custom_schema', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToColumn
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri", "Value")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumn()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToColumnUsingExtension
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
#pragma warning disable S3459 // Unassigned members should be removed
			public int Id { get; set; }
#pragma warning restore S3459 // Unassigned members should be removed
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumnUsingExtension()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>(builder =>
			{
				builder.Entity<TargetEntity>().Property(x => x.Id).AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Value" } });
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToCollection
	{
		[Table("T_Masters")]
		public class SourceMaster
		{
			public int Id { get; set; }

			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class SourceDetail
		{
			public int Id { get; set; }
		}

		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri")]
			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToCollection()
		{
			var source = new EndToEndTestDbContext<SourceMaster>();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToColumnQuoting
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri", "Val'ue")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToColumnQuoting()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Val''ue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyFromColumn
	{
		[Table("Table")]
		private class SourceEntity
		{
			[TestExtendedProperties("Jiri", "Value")]
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyFromColumn()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyFromCollection
	{
		[Table("T_Masters")]
		public class SourceMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri")]
			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class SourceDetail
		{
			public int Id { get; set; }
		}

		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyFromCollection()
		{
			var source = new EndToEndTestDbContext<SourceMaster>();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[T_Masters]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Test_Details_FooBar', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class ChangingPropertyOnColumn
	{
		[Table("Table")]
		private class SourceEntity
		{
			[TestExtendedProperties("Jiri", "OldValue")]
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri", "NewValue")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingPropertyOnColumn()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class ChangingPropertyOnCollection
	{
		[Table("T_Masters")]
		public class SourceMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri")]
			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class SourceDetail
		{
			public int Id { get; set; }
		}

		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri2")]
			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingPropertyOnCollection()
		{
			var source = new EndToEndTestDbContext<SourceMaster>();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri2', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingColumnWithProperty
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
			[TestExtendedProperties("Jiri", "Value")]
			public int Column { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingColumnWithProperty()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Column';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class AddingColumnWithoutProperty
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
			public int Column { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingColumnWithoutProperty()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
		}
	}

	[TestClass]
	public class AddingCollectionWithProperty
	{
		[Table("T_Masters")]
		public class SourceMaster
		{
			public int Id { get; set; }
		}

		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri")]
			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingCollectionWithProperty()
		{
			var source = new EndToEndTestDbContext<SourceMaster>();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(3, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingCollectionWithoutProperty
	{
		[Table("T_Masters")]
		public class SourceMaster
		{
			public int Id { get; set; }
		}

		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[ForeignKey("Column")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingCollectionWithoutProperty()
		{
			var source = new EndToEndTestDbContext<SourceMaster>();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
		}
	}

	[TestClass]
	public class CreatingTableWithPropertyOnTable
	{
		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithPropertyOnTable()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class CreatingTableWithoutPropertyOnTable
	{
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithoutPropertyOnTable()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
		}
	}

	[TestClass]
	public class CreatingTableWithPropertyOnColumn
	{
		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri", "Value")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithPropertyOnColumn()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class CreatingTableWithPropertyOnCollection
	{
		[Table("T_Masters")]
		public class TargetMaster
		{
			public int Id { get; set; }

			[CollectionTestExtendedProperties(FooBar = "Jiri")]
			public List<TargetDetail> Details { get; set; }
		}

		[Table("T_Details")]
		public class TargetDetail
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithPropertyOnCollection()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetMaster>();
			var migrations = source.Migrate(target);

			Assert.HasCount(4, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class CreatingTableWithoutPropertyOnColumn
	{
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithoutPropertyOnColumn()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
		}
	}

	[TestClass]
	public class CreatingTableWithTwoPropertiesOnColumn
	{
		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri1", "ValueA", "Jiri2", "ValueB")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithTwoPropertiesOnColumn()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(3, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri1', @value=N'ValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri2', @value=N'ValueB', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[2].CommandText);
		}
	}

	[TestClass]
	public class DroppingTableWithPropertyOnTable
	{
		[TestExtendedProperties("Jiri", "Value")]
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreatingTableWithPropertyOnTable()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			StringAssert.DoesNotMatch(migrations[0].CommandText, new Regex("EXEC sys.sp_dropextendedproperty"));
		}
	}

	[TestClass]
	public class ChangingTwoPropertiesOnColumn
	{
		[Table("Table")]
		private class SourceEntity
		{
			[TestExtendedProperties("Jiri1", "OldValueA", "Jiri2", "OldValueB")]
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri1", "NewValueA", "Jiri2", "NewValueB")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingTwoPropertiesOnColumn()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(3, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri1', @value=N'NewValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri2', @value=N'NewValueB', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[2].CommandText);
		}
	}

	[TestClass]
	public class ChangingAndRemovingTwoPropertiesOnColumn
	{
		[Table("Table")]
		private class SourceEntity
		{
			[TestExtendedProperties("Jiri1", "OldValueA", "Jiri2", "OldValueB")]
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			[TestExtendedProperties("Jiri1", "NewValueA")]
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingAndRemovingTwoPropertiesOnColumn()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(3, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_dropextendedproperty @name=N'Jiri2', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[1].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri1', @value=N'NewValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id';
",
				migrations[2].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyToModel
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyToModel()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>(builder =>
			{
				builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingTwoPropertiesToModel
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingTwoPropertiesToModel()
		{
			var source = new EndToEndTestDbContext<SourceEntity>();
			var target = new EndToEndTestDbContext<TargetEntity>(builder =>
			{
				builder.AddExtendedProperties(new Dictionary<string, string>()
				{
					{ "Jiri", "Model" },
					{ "Scott", "Hanselman" }
				});
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model';
",
				migrations[0].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Scott', @value=N'Hanselman';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class UpdatingPropertyOnModel
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyOnModel()
		{
			var source = new EndToEndTestDbContext<SourceEntity>(builder =>
			{
				builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "OldValue" } });
			});
			var target = new EndToEndTestDbContext<TargetEntity>(builder =>
			{
				builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "NewValue" } });
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyFromModel
	{
		[Table("Table")]
		private class SourceEntity
		{
			public int Id { get; set; }
		}

		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyFromModel()
		{
			var source = new EndToEndTestDbContext<SourceEntity>(builder =>
			{
				builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
			});
			var target = new EndToEndTestDbContext<TargetEntity>();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_dropextendedproperty @name=N'Jiri';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class CreateDatabaseWithProperty
	{
		// this cannot be generated by migrations so I'm creating the operation manually
		// for more info see XxxRelationalDatabaseCreator

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_CreateDatabaseWithProperty()
		{
			var operation = new SqlServerCreateDatabaseOperation()
			{
				Name = "Dummy"
			};
			operation.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
			var migrations = Migrate(new[] { operation });

			Assert.HasCount(3, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model';
",
				migrations[2].CommandText);
		}
	}

	[TestClass]
	public class NoFKPropertyInCodeNoPropertyInfo
	{
		[Table("Table")]
		private class TargetEntity
		{
			public int Id { get; set; }
			public ICollection<TargetEntity> Sources { get; set; }
		}

		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_NoFKPropertyInCodeNoPropertyInfo()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext<TargetEntity>();
			// should not throw
			source.Migrate(target);
		}
	}

	[TestClass]
	public class AddingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "TYPE", "Name"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class UpdatingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "OldValue" }
				}, "TYPE", "Name"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "NewValue" }
				}, "TYPE", "Name"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "TYPE", "Name"));
			});
			var target = new EndToEndTestDbContext();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[Name]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class ChangingObjectNameExtraDatabaseObjectsForExtraDatabaseObject
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingObjectNameExtraDatabaseObjectsForExtraDatabaseObject()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "TYPE", "OldName"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "TYPE", "NewName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[OldName]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'OldName';
END
", migrations[0].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'NewName';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class ChangingObjectTypeExtraDatabaseObjectsForExtraDatabaseObject
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_ChangingObjectTypeExtraDatabaseObjectsForExtraDatabaseObject()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "OLD_TYPE", "Name"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "NEW_TYPE", "Name"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(2, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[Name]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'OLD_TYPE', @level1name=N'Name';
END
", migrations[0].CommandText);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'NEW_TYPE', @level1name=N'Name';
",
				migrations[1].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyUsingExtraDatabaseObjectsForProcedure
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyUsingExtraDatabaseObjectsForProcedure()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "ProcedureName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class UpdatingPropertyUsingExtraDatabaseObjectsForProcedure
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyUsingExtraDatabaseObjectsForProcedure()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
				{
					{ "Jiri", "OldValue" }
				}, "ProcedureName"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
				{
					{ "Jiri", "NewValue" }
				}, "ProcedureName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyUsingExtraDatabaseObjectsForProcedure
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyUsingExtraDatabaseObjectsForProcedure()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "ProcedureName"));
			});
			var target = new EndToEndTestDbContext();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[ProcedureName]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyUsingExtraDatabaseObjectsForView
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyUsingExtraDatabaseObjectsForView()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "ViewName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class UpdatingPropertyUsingExtraDatabaseObjectsForView
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyUsingExtraDatabaseObjectsForView()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
				{
					{ "Jiri", "OldValue" }
				}, "ViewName"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
				{
					{ "Jiri", "NewValue" }
				}, "ViewName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyUsingExtraDatabaseObjectsForView
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyUsingExtraDatabaseObjectsForView()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "ViewName"));
			});
			var target = new EndToEndTestDbContext();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[ViewName]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName';
END
", migrations[0].CommandText);
		}
	}

	[TestClass]
	public class AddingPropertyUsingExtraDatabaseObjectsForFunction
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_AddingPropertyUsingExtraDatabaseObjectsForFunction()
		{
			var source = new EndToEndTestDbContext();
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "FunctionName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class UpdatingPropertyUsingExtraDatabaseObjectsForFunction
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_UpdatingPropertyUsingExtraDatabaseObjectsForFunction()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
				{
					{ "Jiri", "OldValue" }
				}, "FunctionName"));
			});
			var target = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
				{
					{ "Jiri", "NewValue" }
				}, "FunctionName"));
			});
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(
				@"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName';
",
				migrations[0].CommandText);
		}
	}

	[TestClass]
	public class RemovingPropertyUsingExtraDatabaseObjectsForFunction
	{
		[TestMethod]
		public void ExtendedPropertiesDbMigrations_EndToEnd_RemovingPropertyUsingExtraDatabaseObjectsForFunction()
		{
			var source = new EndToEndTestDbContext(builder =>
			{
				builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
				{
					{ "Jiri", "Value" }
				}, "FunctionName"));
			});
			var target = new EndToEndTestDbContext();
			var migrations = source.Migrate(target);

			Assert.HasCount(1, migrations);
			Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[FunctionName]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName';
END
", migrations[0].CommandText);
		}
	}

	private static IReadOnlyList<MigrationCommand> Migrate(IReadOnlyList<MigrationOperation> diff)
	{
		using (var db = new TestDbContext())
		{
			var generator = db.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(diff, db.Model);
		}
	}
}