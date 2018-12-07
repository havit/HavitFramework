using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
	public class ViewEndToEndTests
    {
        [TestClass]
        public class AddingView
        {
            [Table("Dummy")]
            private class DummySource
            {
                public int Id { get; set; }
            }

            [Table("Dummy")]
            private class DummyTarget
            {
                public int Id { get; set; }
            }

            [TestMethod]
            public void Test()
            {
                var procedure = "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END";

                var source = new EndToEndDbContext<DummySource>();
                var target = new EndToEndDbContext<DummyTarget>(builder => builder.HasAnnotation("View:GetTables", procedure));
                var migrations = Generate(source.Model, target.Model);

                Assert.AreEqual(1, migrations.Count);
                Assert.AreEqual(
                    procedure,
                    migrations[0].CommandText);
            }
        }

        [TestClass]
        public class ModifyingView
        {
            [Table("Dummy")]
            private class DummySource
            {
                public int Id { get; set; }
            }

            [Table("Dummy")]
            private class DummyTarget
            {
                public int Id { get; set; }
            }

            [TestMethod]
            public void Test()
            {
                var source = new EndToEndDbContext<DummySource>(builder => builder.HasAnnotation("View:GetTables", "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END"));
                var newProcedure = "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] WHERE schema_id = 1 END";
	            var newProcedureAlter = "ALTER VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] WHERE schema_id = 1 END";
				var target = new EndToEndDbContext<DummyTarget>(builder => builder.HasAnnotation("View:GetTables", newProcedure));
                var migrations = Generate(source.Model, target.Model);

                Assert.AreEqual(1, migrations.Count);
                Assert.AreEqual(
                    newProcedureAlter,
                    migrations[0].CommandText);
            }
        }

        [TestClass]
        public class DeletingView
        {
            [Table("Dummy")]
            private class DummySource
            {
                public int Id { get; set; }
            }

            [Table("Dummy")]
            private class DummyTarget
            {
                public int Id { get; set; }
            }

            [TestMethod]
            public void Test()
            {
                var source = new EndToEndDbContext<DummySource>(builder => builder.HasAnnotation("View:GetTables", "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END"));
                var target = new EndToEndDbContext<DummyTarget>();
                var migrations = Generate(source.Model, target.Model);

                Assert.AreEqual(1, migrations.Count);
                Assert.AreEqual(
                    "DROP VIEW [GetTables]",
                    migrations[0].CommandText);
            }
        }

	    [TestClass]
	    public class ViewWithMsDescriptionExtendedProperty
		{
			[Attach(nameof(Invoice))]
			public class InvoiceViews : ViewDbInjector
			{
				/// <summary>
				/// Gets all unpaid invoices.
				/// </summary>
				public ViewDbInjection UnpaidInvoices()
				{
					return new ViewDbInjection { CreateSql = "", ViewName = nameof(UnpaidInvoices) };
				}
			}

			[Table("Dummy")]
			private class Invoice
			{
				public int Id { get; set; }
			}

		    //[TestMethod]
			// TODO: support for XML comments / MS_Description on views
		    public void Test()
		    {
			    var source = new EndToEndDbInjectionsDbContext<Invoice>();
			    var model = source.Model;

			    IDictionary<string, string> extendedProperties = model.GetExtendedProperties();
			    Assert.AreEqual("Gets all unpaid invoices.", extendedProperties.FirstOrDefault(a => a.Key.EndsWith("MS_Description")).Value?.Trim());
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

            protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
            {
                base.CustomizeModelCreating(modelBuilder);
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


	    private class EndToEndDbInjectionsDbContext<TEntity> : EndToEndDbContext
		    where TEntity : class
	    {
		    protected override void ModelCreatingCompleting(ModelBuilder modelBuilder)
		    {
				RegisterDbInjections(modelBuilder, this.GetType().Assembly);
		    }

		    public DbSet<TEntity> Entities { get; }
	    }
	}
}