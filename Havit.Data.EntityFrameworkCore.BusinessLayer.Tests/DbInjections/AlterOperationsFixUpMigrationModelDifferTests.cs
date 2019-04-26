using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
    public class AlterOperationsFixUpMigrationModelDifferTests
    {
        [TestClass]
        public class AlterDatabase
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
            public void AlterOperationsFixUpMigrationModelDiffer_AlterDatabase_NoChange_NoMigration()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA"));
                var target = new EndToEndTestDbContext<DummyTarget>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA"));
                var operations = source.Diff(target);

                Assert.AreEqual(0, operations.Count);
            }

            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterDatabase_TwoAnnotationsOneChanged_OneOperation()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyTarget>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);
                Assert.IsInstanceOfType(operations[0], typeof(AlterDatabaseOperation));
            }

            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterDatabase_TwoAnnotationsOneChanged_OneOperationWithOnlyOneCurrentAnnotation()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyTarget>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);

                var operation = (AlterDatabaseOperation)operations[0];
                Assert.AreEqual(1, operation.GetAnnotations().Count());
                Assert.AreEqual("Annotation2", operation.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB_amended", operation.GetAnnotations().First().Value);
                Assert.AreEqual("Annotation2", operation.OldDatabase.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB", operation.OldDatabase.GetAnnotations().First().Value);
            }

            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterDatabase_TwoAnnotationsOneDeleted_OneOperationWithOnlyOneOldAnnotation()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyTarget>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);

                var operation = (AlterDatabaseOperation)operations[0];
                Assert.AreEqual(0, operation.GetAnnotations().Count());
                Assert.AreEqual(1, operation.OldDatabase.GetAnnotations().Count());
                Assert.AreEqual("Annotation2", operation.OldDatabase.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB", operation.OldDatabase.GetAnnotations().First().Value);
            }

            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterDatabase_TwoAnnotationsOneAdded_OneOperationWithOnlyOneCurrentAnnotation()
            {
                var source = new EndToEndTestDbContext<DummySource>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyTarget>(builder =>
                    builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB").HasAnnotation("Annotation3", "Something"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);

                var operation = (AlterDatabaseOperation)operations[0];
                Assert.AreEqual(1, operation.GetAnnotations().Count());
                Assert.AreEqual(0, operation.OldDatabase.GetAnnotations().Count());
                Assert.AreEqual("Annotation3", operation.GetAnnotations().First().Name);
                Assert.AreEqual("Something", operation.GetAnnotations().First().Value);
            }
        }

        private class EndToEndTestDbContext<TEntity> : Tests.EndToEndTestDbContext<TEntity>
            where TEntity : class
        {
            public EndToEndTestDbContext(Action<ModelBuilder> onModelCreating = null)
                : base(onModelCreating)
            { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

                builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>()
                    .WithAnnotationProvider<AllAnnotationsMigrationsAnnotationProvider>());
                builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<DbInjectionsExtension>().WithConsolidateStatementsForMigrationsAnnotationsForModel(true));
            }
        }

        private class AllAnnotationsMigrationsAnnotationProvider : Microsoft.EntityFrameworkCore.Migrations.MigrationsAnnotationProvider
        {
            public AllAnnotationsMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies) : base(dependencies)
            {
            }

            public override IEnumerable<IAnnotation> For(IModel model)
            {
                return model.GetAnnotations();
            }
        }
    }
}