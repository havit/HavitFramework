using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Havit.Business.CodeMigrations.Tests.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.CodeMigrations.Tests.StoredProcedures
{
    public class StoredProceduresEndToEndTests
    {
        [TestClass]
        public class AddingStoredProcedure
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
                var procedure = "CREATE OR ALTER PROCEDURE [dbo].[GetTables]() AS BEGIN SELECT * FROM [sys].[tables] END";

                var source = new EndToEndDbContext<DummySource>();
                var target = new EndToEndDbContext<DummyTarget>(builder => builder.HasAnnotation("StoredProcedure:GetTables", procedure));
                var migrations = Generate(source.Model, target.Model);

                Assert.AreEqual(1, migrations.Count);
                Assert.AreEqual(
                    procedure,
                    migrations[0].CommandText);
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