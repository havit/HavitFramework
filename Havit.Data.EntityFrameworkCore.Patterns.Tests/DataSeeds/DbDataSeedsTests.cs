using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Services.TimeServices;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds;

public class DbDataSeedsTests
{
	private static DbDataSeedRunner GetDefaultDataSeedRunner(IDbContext dbContext, params IDataSeed[] dataSeedsParams)
	{
		IEnumerable<IDataSeed> dataSeeds = new List<IDataSeed>(dataSeedsParams);
		IDataSeedRunDecision dataSeedRunDecision = new AlwaysRunDecision();

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunnerMock = new Mock<IBeforeCommitProcessorsRunner>(MockBehavior.Strict);
		mockBeforeCommitProcessorsRunnerMock.Setup(m => m.Run(It.IsAny<Changes>()));

		Mock<IEntityValidationRunner> mockEntityValidationRunnerMock = new Mock<IEntityValidationRunner>(MockBehavior.Strict);
		mockEntityValidationRunnerMock.Setup(m => m.Validate(It.IsAny<Changes>()));

		Mock<IEntityCacheDependencyManager> entityCacheDependencyManagerMock = new Mock<IEntityCacheDependencyManager>(MockBehavior.Strict);
		entityCacheDependencyManagerMock.Setup(m => m.PrepareCacheInvalidation(It.IsAny<Changes>())).Returns(new CacheInvalidationOperation(() => { }));

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(dbContext, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManagerMock.Object, mockBeforeCommitProcessorsRunnerMock.Object, mockEntityValidationRunnerMock.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		IDataSeedPersister dataSeedPersister = new DbDataSeedPersister(dbContext, dbUnitOfWork);

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersister);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		return new DbDataSeedRunner(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactoryMock.Object, dbContext);
	}

	[TestClass]
	public class CanInsertNewItemsWithGeneratedKey
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_CanInsertNewItemsWithKeyGeneration()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithNullablePropertySeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}

			// Assert
			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithNullableProperty>().AsQueryable(queryTag: this.GetType().Name);
				Assert.AreEqual(3, items.Count());
			}
		}

		private class ItemWithNullablePropertySeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(new ItemWithNullableProperty { NullableValue = 1 },
						new ItemWithNullableProperty { NullableValue = 2 },
						new ItemWithNullableProperty { NullableValue = 6 })
					.PairBy(s => s.NullableValue));
			}
		}
	}

	[TestClass]
	public class CanInsertNewItemsWithoutUpdate
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_CanInsertNewItemsWithoutUpdate()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}

			// Assert
			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithNullableProperty>().AsQueryable(queryTag: this.GetType().Name);

				// Assert that all items ARE deleted (property is not ignored) in INSERT even if it is set by ExcludeUpdateExpressions
				Assert.AreEqual(3, items.Where(i => i.NullableValue != null).Count());
			}
		}

		private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(new ItemWithNullableProperty() { NullableValue = 1, RequiredValue = 1 },
						new ItemWithNullableProperty() { NullableValue = 2, RequiredValue = 2 },
						new ItemWithNullableProperty() { NullableValue = 3, RequiredValue = 3 })
					.PairBy(s => s.RequiredValue)
					.ExcludeUpdate(s => s.NullableValue));
			}
		}
	}

	[TestClass]
	public class AreExcludedUpdatePropertiesSetByExcludeUpdateExpressions
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_AreExcludedUpdatePropertiesSetByExcludeUpdateExpressions()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "B", Deleted = null });
				dbContext.SaveChanges();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}

			// Assert that item is NOT updated, because of ExcludeUpdateExpressions
			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithDeleted>().AsQueryable(queryTag: this.GetType().Name);
				Assert.IsTrue(items.All(item => item.Deleted == null));
			}
		}

		private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(
						new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
						new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now })
					.PairBy(s => s.Symbol)
					.ExcludeUpdate(s => s.Deleted));
			}
		}
	}

	[TestClass]
	public class AreExcludedUpdatePropertiesSetByWithoutUpdate
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_AreExcludedUpdatePropertiesSetByWithoutUpdate()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "B", Deleted = null });
				dbContext.SaveChanges();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}

			// Assert that items ARE NOT updated, because of WithoutUpdate()
			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithDeleted>().AsQueryable(queryTag: this.GetType().Name);
				Assert.AreEqual(true, items.All(item => item.Deleted == null));
			}
		}

		private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(
						new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
						new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now })
					.PairBy(s => s.Symbol)
					.WithoutUpdate());
			}
		}
	}

	[TestClass]
	public class ItemsArePairedAndUpdated
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_ItemsArePairedAndUpdated()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now });
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now });
				dbContext.SaveChanges();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}

			// Assert that ALL items ARE paired and updated

			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithDeleted>().AsQueryable(queryTag: this.GetType().Name);

				Assert.AreEqual(2, items.Count());
				Assert.IsTrue(items.All(item => item.Deleted == null));
			}
		}

		private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(
						new ItemWithDeleted() { Symbol = "A", Deleted = null },
						new ItemWithDeleted() { Symbol = "B", Deleted = null })
					.PairBy(s => s.Symbol));

			}
		}
	}

	[TestClass]
	public class CanMixAndUpdateNewItems
	{
		[TestMethod]
		public void DataSeedRunner_SeedData_CanMixAndUpdateNewItems()
		{
			// Arrange
			using (IDbContext dbContext = new TestDbContext())
			{
				dbContext.Database.DropCreate();

				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
				dbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now });
				dbContext.SaveChanges();
			}

			// Act
			using (IDbContext dbContext = new TestDbContext())
			{
				DbDataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());
				dataSeedRunner.SeedData<DefaultProfile>();
			}


			// Assert that first two items are updated and new two items are created
			using (IDbContext dbContext = new TestDbContext())
			{
				var items = dbContext.Set<ItemWithDeleted>().AsQueryable(queryTag: this.GetType().Name).OrderBy(s => s.Id).ToArray();

				Assert.AreEqual(4, items.Length);
				Assert.AreEqual(true, items.Single(item => item.Symbol == "A").Deleted != null);
				Assert.AreEqual(true, items.Single(item => item.Symbol == "B").Deleted != null);
				Assert.AreEqual(true, items.Single(item => item.Symbol == "C").Deleted == null);
				Assert.AreEqual(true, items.Single(item => item.Symbol == "D").Deleted != null);
			}
		}

		private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
		{
			public override void SeedData()
			{
				Seed(For(new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
					new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now },
					new ItemWithDeleted() { Symbol = "C", Deleted = null },
					new ItemWithDeleted() { Symbol = "D", Deleted = DateTime.Now })
					.PairBy(s => s.Symbol));
			}
		}
	}

}
