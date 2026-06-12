using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Internal;

#pragma warning disable EF1001 // Internal EF Core API usage.
[TestClass]
public class DbSetInternalTests
{
	[TestMethod]
	public void DbSetInternal_FindTracked_SinglePrimaryKey_EntityIsNotTracked()
	{
		// Arrange
		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(new TestDbContext());

		// Act
		SinglePrimaryKeyEntity trackedEntity = dbSetInternal.FindTracked((object)1);

		// Assert
		Assert.IsNull(trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_FindTracked_SinglePrimaryKey_EntityIsTracked()
	{
		// Arrange
		SinglePrimaryKeyEntity entity = new SinglePrimaryKeyEntity { Id = 1 };

		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(new TestDbContext());
		dbSetInternal.Attach(entity);

		// Act
		SinglePrimaryKeyEntity trackedEntity = dbSetInternal.FindTracked((object)entity.Id);

		// Assert
		Assert.AreSame(entity, trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_FindTracked_CompositePrimaryKey_EntityIsNotTracked()
	{
		// Arrange
		DbSetInternal<CompositePrimaryKeyEntity> dbSetInternal = new DbSetInternal<CompositePrimaryKeyEntity>(new TestDbContext());

		// Act
		CompositePrimaryKeyEntity trackedEntity = dbSetInternal.FindTracked((object)1, (object)2);

		// Assert
		Assert.IsNull(trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_FindTracked_CompositePrimaryKey_EntityIsTracked()
	{
		// Arrange
		CompositePrimaryKeyEntity entity = new CompositePrimaryKeyEntity { Id1 = 1, Id2 = 2 };

		DbSetInternal<CompositePrimaryKeyEntity> dbSetInternal = new DbSetInternal<CompositePrimaryKeyEntity>(new TestDbContext());
		dbSetInternal.Attach(entity);

		// Act
		CompositePrimaryKeyEntity trackedEntity = dbSetInternal.FindTracked((object)entity.Id1, (object)entity.Id2);

		// Assert
		Assert.AreSame(entity, trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_FindTrackedTyped_EntityIsNotTracked()
	{
		// Arrange
		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(new TestDbContext());

		// Act
		SinglePrimaryKeyEntity trackedEntity = dbSetInternal.FindTrackedTyped(1);

		// Assert
		Assert.IsNull(trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_FindTrackedTyped_EntityIsTracked()
	{
		// Arrange
		SinglePrimaryKeyEntity entity = new SinglePrimaryKeyEntity { Id = 1 };

		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(new TestDbContext());
		dbSetInternal.Attach(entity);

		// Act
		SinglePrimaryKeyEntity trackedEntity = dbSetInternal.FindTrackedTyped(entity.Id);

		// Assert
		Assert.AreSame(entity, trackedEntity);
	}

	[TestMethod]
	public void DbSetInternal_AsQueryable_WithoutQueryTag_ReturnsDbSet()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(dbContext);

		// Act
		IQueryable<SinglePrimaryKeyEntity> queryableForNull = dbSetInternal.AsQueryable(null);
		IQueryable<SinglePrimaryKeyEntity> queryableForEmpty = dbSetInternal.AsQueryable(String.Empty);

		// Assert
		// Bez query tagu vrací přímo DbSet (EF cachuje DbSet per typ, jde tedy o tutéž instanci).
		Assert.AreSame(dbContext.Set<SinglePrimaryKeyEntity>(), queryableForNull);
		Assert.AreSame(dbContext.Set<SinglePrimaryKeyEntity>(), queryableForEmpty);
	}

	[TestMethod]
	public void DbSetInternal_AsQueryable_WithQueryTag_ReturnsTaggedQuery()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		DbSetInternal<SinglePrimaryKeyEntity> dbSetInternal = new DbSetInternal<SinglePrimaryKeyEntity>(dbContext);

		// Act
		IQueryable<SinglePrimaryKeyEntity> queryable = dbSetInternal.AsQueryable("MyQueryTag");

		// Assert
		// S query tagem nevrací přímo DbSet, ale otagovaný dotaz, jehož tag je součástí expression tree.
		Assert.AreNotSame(dbContext.Set<SinglePrimaryKeyEntity>(), queryable);
		Assert.Contains("MyQueryTag", queryable.Expression.ToString());
	}

	public class TestDbContext : DbContext
	{
		private readonly string _databaseName;

		public TestDbContext([CallerMemberName] string databaseName = default)
		{
			_databaseName = databaseName;
		}

		protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseInMemoryDatabase(_databaseName);
		}
		protected override void CustomizeModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Entity<SinglePrimaryKeyEntity>().HasKey(entity => entity.Id);
			modelBuilder.Entity<CompositePrimaryKeyEntity>().HasKey(entity => new { entity.Id1, entity.Id2 });
		}
	}

	public class SinglePrimaryKeyEntity
	{
		public int Id { get; set; }
	}

	public class CompositePrimaryKeyEntity
	{
		public int Id1 { get; set; }
		public int Id2 { get; set; }
	}
}
#pragma warning restore EF1001 // Internal EF Core API usage.
