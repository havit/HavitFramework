using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions
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
            public void ViewModelExtensions_EndToEnd_AddingView()
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
            public void ViewModelExtensions_EndToEnd_ModifyingView()
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
            public void ViewModelExtensions_EndToEnd_DeletingView()
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
	}
}