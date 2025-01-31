using Havit.Data.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
	public void DbSetInternal_FindTrackedd_CompositePrimaryKey_EntityIsTracked()
	{
		// Arrange
		CompositePrimaryKeyEntity entity = new CompositePrimaryKeyEntity { Id1 = 1, Id2 = 2 };

		DbSetInternal<CompositePrimaryKeyEntity> dbSetInternal = new DbSetInternal<CompositePrimaryKeyEntity>(new TestDbContext());
		dbSetInternal.Attach(entity);

		// Act
		CompositePrimaryKeyEntity trackedEntity = dbSetInternal.FindTracked((object)entity.Id1, (object)entity.Id2);
		Assert.AreSame(entity, trackedEntity);
	}

	public class TestDbContext : DbContext
	{
		protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseInMemoryDatabase(this.GetType().Name);
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
