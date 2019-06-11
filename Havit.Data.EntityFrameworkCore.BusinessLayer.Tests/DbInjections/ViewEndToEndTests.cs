using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections.Views;
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
            public void ViewDbInjections_EndToEnd_AddingView()
            {
                var procedure = "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END";

                var source = new EndToEndTestDbContext<DummySource>();
                var target = new EndToEndTestDbContext<DummyTarget>(builder => builder.HasAnnotation("View:GetTables", procedure));
                var migrations = source.Migrate(target);

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
            public void ViewDbInjections_EndToEnd_ModifyingView()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder => builder.HasAnnotation("View:GetTables", "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END"));
                var newProcedure = "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] WHERE schema_id = 1 END";
	            var newProcedureAlter = "ALTER VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] WHERE schema_id = 1 END";
				var target = new EndToEndTestDbContext<DummyTarget>(builder => builder.HasAnnotation("View:GetTables", newProcedure));
                var migrations = source.Migrate(target);

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
            public void ViewDbInjections_EndToEnd_DeletingView()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder => builder.HasAnnotation("View:GetTables", "CREATE VIEW [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END"));
                var target = new EndToEndTestDbContext<DummyTarget>();
                var migrations = source.Migrate(target);

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
		    public void ViewDbInjections_EndToEnd_ViewWithMsDescriptionExtendedProperty()
		    {
			    var source = new EndToEndTestDbInjectionsDbContext<Invoice>(typeof(InvoiceViews));
			    var model = source.Model;

			    IDictionary<string, string> extendedProperties = model.GetExtendedProperties();
			    Assert.AreEqual("Gets all unpaid invoices.", extendedProperties.FirstOrDefault(a => a.Key.EndsWith("MS_Description")).Value?.Trim());
		    }
	    }
	}
}