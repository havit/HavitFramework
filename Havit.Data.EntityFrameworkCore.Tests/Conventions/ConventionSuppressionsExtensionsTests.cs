using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Attributes;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Tests.Conventions;

[TestClass]
public class ConventionSuppressionsExtensionsTests
{
	private const string TestCustomConventionIdentifier = nameof(TestCustomConventionIdentifier);

	[TestMethod]
	public void ConventionSuppressionsExtensionsTest_IsConventionSuppressed_ReturnsTrueForSuppressedConventions()
	{
		// Arrange
		DbContext dbContext = new TestDbContext();

		// Act in DbContext

		// Assert
		Assert.IsTrue(dbContext.Model.FindEntityType(typeof(EntityWithSuppression)).IsConventionSuppressed(TestCustomConventionIdentifier));
		Assert.IsTrue(dbContext.Model.FindEntityType(typeof(EntityWithSuppression)).FindProperty(nameof(EntityWithoutSuppression.Value)).IsConventionSuppressed(TestCustomConventionIdentifier));
	}

	[TestMethod]
	public void ConventionSuppressionsExtensionsTest_IsConventionSuppressed_ReturnsFalseForNotSuppressedConventions()
	{
		// Arrange
		DbContext dbContext = new TestDbContext();

		// Act in DbContext

		// Assert
		Assert.IsFalse(dbContext.Model.FindEntityType(typeof(EntityWithoutSuppression)).IsConventionSuppressed(TestCustomConventionIdentifier));
		Assert.IsFalse(dbContext.Model.FindEntityType(typeof(EntityWithoutSuppression)).FindProperty(nameof(EntityWithoutSuppression.Value)).IsConventionSuppressed(TestCustomConventionIdentifier));
	}

	public class TestDbContext : DbContext
	{
		private readonly string _databaseName;

		public TestDbContext([CallerMemberName] string databaseName = default)
		{
			_databaseName = databaseName;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseInMemoryDatabase(_databaseName);
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			// Act
			modelBuilder.Entity<EntityWithoutSuppression>();
			modelBuilder.Entity<EntityWithSuppression>();
		}
	}

	[SuppressConvention(TestCustomConventionIdentifier)]
	public class EntityWithSuppression
	{
		public int Id { get; set; }

		[SuppressConvention(TestCustomConventionIdentifier)]
		public string Value { get; set; }
	}

	public class EntityWithoutSuppression
	{
		public int Id { get; set; }
		public string Value { get; set; }
	}
}