using Microsoft.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.Conventions
{
	[TestClass]
	public class ConventionSuppressionsExtensionsTests
	{
		[TestMethod]
		public void ConventionSuppressionsExtensionsTest_IsConventionSuppressed_ReturnsTrueForSuppressedConventions()
		{
			// Arrange
			DbContext dbContext = new TestDbContext();

			// Act in DbContext

			// Assert
			Assert.IsTrue(dbContext.Model.FindEntityType(typeof(EntityWithSuppression)).IsConventionSuppressed<TestConvention>());
			Assert.IsTrue(dbContext.Model.FindEntityType(typeof(EntityWithSuppression)).FindProperty(nameof(EntityWithoutSuppression.Value)).IsConventionSuppressed<TestConvention>());
		}

		[TestMethod]
		public void ConventionSuppressionsExtensionsTest_IsConventionSuppressed_ReturnsFalseForNotSuppressedConventions()
		{
			// Arrange
			DbContext dbContext = new TestDbContext();

			// Act in DbContext

			// Assert
			Assert.IsFalse(dbContext.Model.FindEntityType(typeof(EntityWithoutSuppression)).IsConventionSuppressed<TestConvention>());
			Assert.IsFalse(dbContext.Model.FindEntityType(typeof(EntityWithoutSuppression)).FindProperty(nameof(EntityWithoutSuppression.Value)).IsConventionSuppressed<TestConvention>());
		}

		public class TestDbContext : DbContext
		{
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);
				optionsBuilder.UseInMemoryDatabase(nameof(TestDbContext));
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);

				// Act
				modelBuilder.Entity<EntityWithoutSuppression>();
				modelBuilder.Entity<EntityWithSuppression>(eb =>
				{
					eb.HasConventionSuppressed<TestConvention>();
					eb.Property(p => p.Value).HasConventionSuppressed<TestConvention>();
				});				
			}
		}

		public class EntityWithSuppression
		{
			public int Id { get; set; }
			public string Value { get; set; }
		}

		public class EntityWithoutSuppression
		{
			public int Id { get; set; }
			public string Value { get; set; }
		}

		public class TestConvention : IModelConvention
		{
			public void Apply(ModelBuilder modelBuilder)
			{
				// NOOP
			}
		}
	}
}