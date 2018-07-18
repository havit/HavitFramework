using System;
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
			private class AddingPropertyToTableSourceEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[TestExtendedProperty("Jiri", "Value")]
			[Table("Table")]
			private class AddingPropertyToTableTargetEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<AddingPropertyToTableSourceEntity>();
				var target = new EndToEndDbContext<AddingPropertyToTableTargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
					migrations[1].CommandText);
			}
		}

		[TestClass]
		public class RemovingPropertyFromTable
		{
			[Table("Table")]
			[TestExtendedProperty("Jiri", "Value")]
			private class RemovingPropertyFromTableSourceEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[Table("Table")]
			private class RemovingPropertyFromTableTargetEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<RemovingPropertyFromTableSourceEntity>();
				var target = new EndToEndDbContext<RemovingPropertyFromTableTargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_dropextendedproperty @name=N'Jiri', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
					migrations[1].CommandText);
			}
		}

		[TestClass]
		public class ChangingPropertyOnTable
		{
			[Table("Table")]
			[TestExtendedProperty("Jiri", "OldValue")]
			private class ChangingPropertyOnTableSourceEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[Table("Table")]
			[TestExtendedProperty("Jiri", "NewValue")]
			private class ChangingPropertyOnTableTargetEntity
			{
				[Column("Id")]
				public int Id { get; set; }
			}

			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<ChangingPropertyOnTableSourceEntity>();
				var target = new EndToEndDbContext<ChangingPropertyOnTableTargetEntity>();
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(2, migrations.Count);
				Assert.AreEqual(
					"EXEC sys.sp_updateextendedproperty @name=N'Jiri', @value=N'NewValue', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
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
