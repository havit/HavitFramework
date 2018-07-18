﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		private class EndToEndDbContext<TEntity> : TestDbContext
			where TEntity : class
		{
			public DbSet<TEntity> Entities { get; }
		}
	}
}
