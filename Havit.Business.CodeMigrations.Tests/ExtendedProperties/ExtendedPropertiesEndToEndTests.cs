using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
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

			[TestExtendedProperty("Jiri", "Value")]
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
			[TestExtendedProperty("Jiri", "Value")]
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
			[TestExtendedProperty("Jiri", "OldValue")]
			[Table("Table")]
			private class SourceEntity
			{
				public int Id { get; set; }
			}

			[TestExtendedProperty("Jiri", "NewValue")]
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
				[Column("Id")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				[TestExtendedProperty("Jiri", "Value")]
				[Column("Id")]
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
				[Column("Id")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				[Column("Id")]
				public int Id { get; set; }
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
		public class RemovingPropertyFromColumn
		{
			[Table("Table")]
			private class SourceEntity
			{
				[TestExtendedProperty("Jiri", "Value")]
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
		public class ChangingPropertyOnColumn
		{
			[Table("Table")]
			private class SourceEntity
			{
				[TestExtendedProperty("Jiri", "OldValue")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				[TestExtendedProperty("Jiri", "NewValue")]
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
				[TestExtendedProperty("Jiri", "Value")]
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
		public class CreatingTableWithPropertyOnTable
		{
			[TestExtendedProperty("Jiri", "Value")]
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
				[TestExtendedProperty("Jiri", "Value")]
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
				[TestExtendedProperty("Jiri1", "ValueA", "Jiri2", "ValueB")]
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
				[TestExtendedProperty("Jiri1", "OldValueA", "Jiri2", "OldValueB")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				[TestExtendedProperty("Jiri1", "NewValueA", "Jiri2", "NewValueB")]
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
				[TestExtendedProperty("Jiri1", "OldValueA", "Jiri2", "OldValueB")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class TargetEntity
			{
				[TestExtendedProperty("Jiri1", "NewValueA")]
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

		private static IReadOnlyList<MigrationCommand> Generate(IModel source, IModel target)
		{
			using (var db = new TestDbContext())
			{
				var differ = db.GetService<IMigrationsModelDiffer>();
				var generator = db.GetService<IMigrationsSqlGenerator>();
				var diff = differ.GetDifferences(source, target);
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

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				onModelCreating?.Invoke(modelBuilder);
			}
		}

		private class EndToEndDbContext<TEntity> : EndToEndDbContext
			where TEntity : class
		{
			public EndToEndDbContext(Action<ModelBuilder> onModelCreating = null)
				: base(onModelCreating)
			{ }

			public DbSet<TEntity> Entities { get; }
		}
	}
}
