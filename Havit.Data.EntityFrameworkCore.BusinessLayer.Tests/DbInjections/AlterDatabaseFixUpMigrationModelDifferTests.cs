using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
	public class AlterDatabaseFixUpMigrationModelDifferTests
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

		[TestClass]
		public class NoChange_NoMigration
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<DummySource>(builder => 
					builder.HasAnnotation("Annotation1", "ValueA"));
				var target = new EndToEndDbContext<DummyTarget>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA"));
				var migrations = Generate(source.Model, target.Model);

				Assert.AreEqual(0, migrations.Count);
			}
		}

		[TestClass]
		public class TwoAnnotationsOneChanged_OneOperation
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<DummySource>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
				var target = new EndToEndDbContext<DummyTarget>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB_amended"));
				var operations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, operations.Count);
				Assert.IsInstanceOfType(operations[0], typeof(AlterDatabaseOperation));
			}
		}

		[TestClass]
		public class TwoAnnotationsOneChanged_OneOperationWithOnlyOneCurrentAnnotation
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<DummySource>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
				var target = new EndToEndDbContext<DummyTarget>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB_amended"));
				var operations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, operations.Count);

				var operation = (AlterDatabaseOperation)operations[0];
				Assert.AreEqual(1, operation.GetAnnotations().Count());
				Assert.AreEqual("Annotation2", operation.GetAnnotations().First().Name);
				Assert.AreEqual("ValueB_amended", operation.GetAnnotations().First().Value);
				Assert.AreEqual("Annotation2", operation.OldDatabase.GetAnnotations().First().Name);
				Assert.AreEqual("ValueB", operation.OldDatabase.GetAnnotations().First().Value);
			}
		}

		[TestClass]
		public class TwoAnnotationsOneDeleted_OneOperationWithOnlyOneOldAnnotation
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<DummySource>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
				var target = new EndToEndDbContext<DummyTarget>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA"));
				var operations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, operations.Count);

				var operation = (AlterDatabaseOperation)operations[0];
				Assert.AreEqual(0, operation.GetAnnotations().Count());
				Assert.AreEqual(1, operation.OldDatabase.GetAnnotations().Count());
				Assert.AreEqual("Annotation2", operation.OldDatabase.GetAnnotations().First().Name);
				Assert.AreEqual("ValueB", operation.OldDatabase.GetAnnotations().First().Value);
			}
		}

		[TestClass]
		public class TwoAnnotationsOneAdded_OneOperationWithOnlyOneCurrentAnnotation
		{
			[TestMethod]
			public void Test()
			{
				var source = new EndToEndDbContext<DummySource>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB"));
				var target = new EndToEndDbContext<DummyTarget>(builder =>
					builder.HasAnnotation("Annotation1", "ValueA").HasAnnotation("Annotation2", "ValueB").HasAnnotation("Annotation3", "Something"));
				var operations = Generate(source.Model, target.Model);

				Assert.AreEqual(1, operations.Count);

				var operation = (AlterDatabaseOperation)operations[0];
				Assert.AreEqual(1, operation.GetAnnotations().Count());
				Assert.AreEqual(0, operation.OldDatabase.GetAnnotations().Count());
				Assert.AreEqual("Annotation3", operation.GetAnnotations().First().Name);
				Assert.AreEqual("Something", operation.GetAnnotations().First().Value);
			}
		}

		private static IReadOnlyList<MigrationOperation> Generate(IModel source, IModel target)
		{
			using (var db = new EndToEndDbContext())
			{
				var differ = db.GetService<IMigrationsModelDiffer>();
				return differ.GetDifferences(source, target);
			}
		}

		private class EndToEndDbContext : TestDbContext
		{
			private readonly Action<ModelBuilder> onModelCreating;

			public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
			{
				this.onModelCreating = onModelCreating;
			}

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);

				optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<AllAnnotationsMigrationsAnnotationProvider>();
				optionsBuilder.Options.GetExtension<DbInjectionsExtension>().WithOptions(new DbInjectionsOptions { RemoveUnnecessaryStatementsForMigrationsAnnotationsForModel = true });
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