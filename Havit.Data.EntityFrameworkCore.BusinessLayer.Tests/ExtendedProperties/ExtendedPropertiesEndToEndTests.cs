using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>(builder =>
				{
					builder.Entity<TargetEntity>().AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Value" } });
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
					migrations[0].CommandText);
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>(builder =>
				{
					builder.Entity<TargetEntity>().Property(x => x.Id).AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Value" } });
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceMaster>();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Val''ue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceMaster>();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Test_Details_FooBar', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters'",
					migrations[0].CommandText);
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceMaster>();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri2', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Column'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceMaster>();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(3, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceMaster>();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetMaster>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(4, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Test_Details_FooBar', @value=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'T_Masters'",
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(3, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri1', @value=N'ValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
					migrations[1].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri2', @value=N'ValueB', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
					migrations[2].CommandText);
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(3, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri1', @value=N'NewValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
					migrations[1].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri2', @value=N'NewValueB', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(3, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri2', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
					migrations[1].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri1', @value=N'NewValueA', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table', @level2type=N'COLUMN', @level2name=N'Id'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>(builder =>
				{
					builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>();
				var target = new EndToEndDbContext<TargetEntity>(builder =>
				{
					builder.AddExtendedProperties(new Dictionary<string, string>()
					{
						{ "Jiri", "Model" },
						{ "Scott", "Hanselman" }
					});
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model'",
					migrations[0].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Scott', @value=N'Hanselman'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>(builder =>
				{
					builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "OldValue" } });
				});
				var target = new EndToEndDbContext<TargetEntity>(builder =>
				{
					builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "NewValue" } });
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue'",
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
			public void Test()
			{
				var source = new EndToEndDbContext<SourceEntity>(builder =>
				{
					builder.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
				});
				var target = new EndToEndDbContext<TargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class CreateDatabaseWithProperty
		{
			// this cannot be generated by migrations so I'm creating the operation manually
			// for more info see XxxRelationalDatabaseCreator

			[TestMethod]
			public void Test()
			{
				var operation = new SqlServerCreateDatabaseOperation()
				{
					Name = "Dummy"
				};
				operation.AddExtendedProperties(new Dictionary<string, string>() { { "Jiri", "Model" } });
				var migrations = Migrate(new[] { operation });

				Assert.AreEqual(3, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Model'",
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
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext<TargetEntity>();
				// should not throw
				Generate(source.Model, target.Model);
			}
		}

		[TestClass]
		public class AddingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "TYPE", "Name"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class UpdatingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "OldValue" }
					}, "TYPE", "Name"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "NewValue" }
					}, "TYPE", "Name"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class RemovingPropertyUsingExtraDatabaseObjectsForExtraDatabaseObject
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "TYPE", "Name"));
				});
				var target = new EndToEndDbContext();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'Name'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class ChangingObjectNameExtraDatabaseObjectsForExtraDatabaseObject
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "TYPE", "OldName"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "TYPE", "NewName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'OldName'",
					migrations[0].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TYPE', @level1name=N'NewName'",
					migrations[1].CommandText);
			}
		}

		[TestClass]
		public class ChangingObjectTypeExtraDatabaseObjectsForExtraDatabaseObject
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "OLD_TYPE", "Name"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForExtraDatabaseObject(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "NEW_TYPE", "Name"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'OLD_TYPE', @level1name=N'Name'",
					migrations[0].CommandText);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'NEW_TYPE', @level1name=N'Name'",
					migrations[1].CommandText);
			}
		}

		[TestClass]
		public class AddingPropertyUsingExtraDatabaseObjectsForProcedure
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "ProcedureName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class UpdatingPropertyUsingExtraDatabaseObjectsForProcedure
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
					{
						{ "Jiri", "OldValue" }
					}, "ProcedureName"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
					{
						{ "Jiri", "NewValue" }
					}, "ProcedureName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class RemovingPropertyUsingExtraDatabaseObjectsForProcedure
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "ProcedureName"));
				});
				var target = new EndToEndDbContext();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'ProcedureName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class AddingPropertyUsingExtraDatabaseObjectsForView
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "ViewName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class UpdatingPropertyUsingExtraDatabaseObjectsForView
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
					{
						{ "Jiri", "OldValue" }
					}, "ViewName"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
					{
						{ "Jiri", "NewValue" }
					}, "ViewName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class RemovingPropertyUsingExtraDatabaseObjectsForView
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForView(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "ViewName"));
				});
				var target = new EndToEndDbContext();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'ViewName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class AddingPropertyUsingExtraDatabaseObjectsForFunction
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext();
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "FunctionName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class UpdatingPropertyUsingExtraDatabaseObjectsForFunction
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
					{
						{ "Jiri", "OldValue" }
					}, "FunctionName"));
				});
				var target = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
					{
						{ "Jiri", "NewValue" }
					}, "FunctionName"));
				});
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName'",
					migrations[0].CommandText);
			}
		}

		[TestClass]
		public class RemovingPropertyUsingExtraDatabaseObjectsForFunction
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext(builder =>
				{
					builder.Model.AddAnnotations(ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForFunction(new Dictionary<string, string>()
					{
						{ "Jiri", "Value" }
					}, "FunctionName"));
				});
				var target = new EndToEndDbContext();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'FUNCTION', @level1name=N'FunctionName'",
					migrations[0].CommandText);
			}
		}

		private static IReadOnlyList<MigrationCommand> Generate(IModel source, IModel target)
		{
			return Migrate(Diff(source, target));
		}

		private static IReadOnlyList<MigrationOperation> Diff(IModel source, IModel target)
		{
			using (var db = new TestDbContext())
			{
				var differ = db.GetService<IMigrationsModelDiffer>();
				return differ.GetDifferences(source, target);
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

		private class EndToEndDbContext : TestDbContext
		{
			private readonly Action<ModelBuilder> onModelCreating;

			public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
			{
				this.onModelCreating = onModelCreating;
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);
				onModelCreating?.Invoke(modelBuilder);
			}
		}

		private class EndToEndDbContext<TEntity> : EndToEndDbContext
			where TEntity : class
		{
			public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
				: base(onModelCreating)
			{ }

			public DbSet<TEntity> Entities { get; }
		}
	}
}
