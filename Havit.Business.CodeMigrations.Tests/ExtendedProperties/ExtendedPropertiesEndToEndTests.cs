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
	[TestClass]
	public class ExtendedPropertiesEndToEndTests
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
		public void AddingPropertyToTable()
		{
			var source = new EndToEndDbContext<AddingPropertyToTableSourceEntity>();
			var target = new EndToEndDbContext<AddingPropertyToTableTargetEntity>();
			var migrations = Generate(source.Model, target.Model);

			Assert.AreEqual(2, migrations.Count);
			Assert.AreEqual(
				"EXEC sys.sp_addextendedproperty @name=N'Jiri', @value=N'Value', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Table'",
				migrations[1].CommandText);
		}

		private IReadOnlyList<MigrationCommand> Generate(IModel source, IModel target)
		{
			using (var db = new TestDbContext())
			{
				var differ = db.GetService<IMigrationsModelDiffer>();
				var generator = db.GetService<IMigrationsSqlGenerator>();
				var diff =  differ.GetDifferences(source, target);
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
