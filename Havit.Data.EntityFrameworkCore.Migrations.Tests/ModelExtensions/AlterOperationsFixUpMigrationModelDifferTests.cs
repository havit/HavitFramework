using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions
{
    public class AlterOperationsFixUpMigrationModelDifferTests
    {
        private const string TestAnnotationPrefix = "TestAnnotations:";

        [TestClass]
        public class AlterTable
        {
            [Table("Dummy")]
            private class DummyEntity
            {
                public int Id { get; set; }
            }

            /// <summary>
            /// Tests, whether two equal entities (with equal annotations) don't result in migration operation.
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterTable_NoChange_NoMigration()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA"));
                var operations = source.Diff(target);

                Assert.AreEqual(0, operations.Count);
            }

            /// <summary>
            /// Tests, whether changing one annotation on entity results in one migration operation (<see cref="AlterTableOperation"/>.
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterTable_TwoAnnotationsOneChanged_OneOperation()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);
                Assert.IsInstanceOfType(operations[0], typeof(AlterTableOperation));
            }

            /// <summary>
            /// Tests, whether the model differ preserves other properties of <see cref="AlterTableOperation"/>.
            ///
            /// Specifically, checks whether table name from newer model is preserved.
            ///
            /// Does not test result of annotation fix up (done by other tests).
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterTable_TwoAnnotationsOneChanged_OtherPropertiesAreSame()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .ToTable("SourceTable")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .ToTable("TargetTable")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                // Changing table name yields multiple operations (such as recreating PK), we are interested only in AlterTableOperation
                Assert.AreEqual(1, operations.OfType<AlterTableOperation>().Count());

                var operation = operations.OfType<AlterTableOperation>().First();
                Assert.AreEqual("TargetTable", operation.Name);
                Assert.IsNull(operation.Schema);
            }

            /// <summary>
            /// Tests, whether the model differ correctly removes extra annotation from <see cref="AlterTableOperation"/> and <see cref="AlterTableOperation.OldTable"/> annotations.
            ///
            /// Source:
            ///     - Annotation2:ValueA
            ///     - Annotation2:ValueB
            /// Target:
            ///     - Annotation2:ValueA
            ///     - Annotation2:ValueB_amended
            ///
            /// (Annotation Annotation2:ValueA does not have to be in the migration operation)
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterTable_TwoAnnotationsOneChanged_OneOperationWithOnlyOneCurrentAnnotation()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);

                var operation = (AlterTableOperation)operations[0];
                Assert.AreEqual(1, operation.GetAnnotations().Count());
                Assert.AreEqual($"{TestAnnotationPrefix}Annotation2", operation.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB_amended", operation.GetAnnotations().First().Value);
                Assert.AreEqual($"{TestAnnotationPrefix}Annotation2", operation.OldTable.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB", operation.OldTable.GetAnnotations().First().Value);
            }
        }

        [TestClass]
        public class AlterColumn
        {
            [Table("Dummy")]
            private class DummyEntity
            {
                public int Id { get; set; }

                public string MyProperty { get; set; }
            }

            /// <summary>
            /// Tests, whether two equal entities (with equal annotations) don't result in migration operation.
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterColumn_NoChange_NoMigration()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty).HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty).HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA"));
                var operations = source.Diff(target);

                Assert.AreEqual(0, operations.Count);
            }

            /// <summary>
            /// Tests, whether changing one annotation on property results in one migration operation (<see cref="AlterColumnOperation"/>.
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterColumn_TwoAnnotationsOneChanged_OneOperation()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty)
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty)
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);
                Assert.IsInstanceOfType(operations[0], typeof(AlterColumnOperation));
            }

            /// <summary>
            /// Tests, whether the model differ preserves other properties of <see cref="AlterColumnOperation"/>.
            ///
            /// Specifically, checks whether column name from newer model is preserved.
            ///
            /// Does not test result of annotation fix up (done by other tests).
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterColumn_TwoAnnotationsOneChanged_OtherPropertiesAreSame()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty)
                        .HasColumnName("SourceColumn")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>()
                        .Property(x => x.MyProperty)
                        .HasColumnName("TargetColumn")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                // Changing table name yields other operations (such as renaming column), we are interested only in AlterColumnOperation
                Assert.AreEqual(1, operations.OfType<AlterColumnOperation>().Count());

                var operation = operations.OfType<AlterColumnOperation>().First();
                Assert.AreEqual("TargetColumn", operation.Name);
                Assert.AreEqual("Dummy", operation.Table);
                Assert.AreEqual(typeof(string), operation.ClrType);
                Assert.AreEqual(null, operation.ColumnType);
                Assert.AreEqual(true, operation.IsNullable);
                Assert.AreEqual(typeof(string), operation.OldColumn.ClrType);
                Assert.AreEqual(null, operation.OldColumn.ColumnType);
                Assert.AreEqual(true, operation.OldColumn.IsNullable);
                Assert.IsNull(operation.Schema);
            }

            /// <summary>
            /// Tests, whether the model differ correctly removes extra annotation from <see cref="AlterColumnOperation"/> and <see cref="AlterColumnOperation.OldColumn"/> annotations.
            ///
            /// Source:
            ///     - Annotation2:ValueA
            ///     - Annotation2:ValueB
            /// Target:
            ///     - Annotation2:ValueA
            ///     - Annotation2:ValueB_amended
            ///
            /// (Annotation Annotation2:ValueA does not have to be in the migration operation)
            /// </summary>
            [TestMethod]
            public void AlterOperationsFixUpMigrationModelDiffer_AlterColumn_TwoAnnotationsOneChanged_OneOperationWithOnlyOneCurrentAnnotation()
            {
                var source = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty)
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB"));
                var target = new EndToEndTestDbContext<DummyEntity>(builder =>
                    builder.Entity<DummyEntity>().Property(x => x.MyProperty)
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation1", "ValueA")
                        .HasAnnotation($"{TestAnnotationPrefix}Annotation2", "ValueB_amended"));
                var operations = source.Diff(target);

                Assert.AreEqual(1, operations.Count);

                var operation = (AlterColumnOperation)operations[0];
                Assert.AreEqual(1, operation.GetAnnotations().Count());
                Assert.AreEqual($"{TestAnnotationPrefix}Annotation2", operation.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB_amended", operation.GetAnnotations().First().Value);
                Assert.AreEqual($"{TestAnnotationPrefix}Annotation2", operation.OldColumn.GetAnnotations().First().Name);
                Assert.AreEqual("ValueB", operation.OldColumn.GetAnnotations().First().Value);
            }
        }

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
                    .WithAnnotationProvider<TestAnnotationsMigrationsAnnotationProvider>());
                builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<ModelExtensionsExtension>().WithConsolidateStatementsForMigrationsAnnotationsForModel(true));
            }
        }

        private class TestAnnotationsMigrationsAnnotationProvider : Microsoft.EntityFrameworkCore.Migrations.MigrationsAnnotationProvider
        {
            public TestAnnotationsMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies) : base(dependencies)
            {
            }

            public override IEnumerable<IAnnotation> For(IProperty property)
            {
                return property.GetAnnotations().Where(a => a.Name.StartsWith(TestAnnotationPrefix));
            }

            public override IEnumerable<IAnnotation> For(IEntityType entityType)
            {
                return entityType.GetAnnotations().Where(a => a.Name.StartsWith(TestAnnotationPrefix));
            }

            public override IEnumerable<IAnnotation> For(IModel model)
            {
                return model.GetAnnotations();
            }
        }
    }
}