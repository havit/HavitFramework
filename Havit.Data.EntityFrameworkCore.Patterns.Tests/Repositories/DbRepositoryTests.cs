using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders.Fakes;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Repositories;

[TestClass]
public class DbRepositoryTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void DbRepository_GetObject_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Assert
		Assert.ThrowsExactly<Havit.Data.Patterns.Exceptions.ObjectNotFoundException>(() =>
		{
			// Act
			repository.GetObject(maxId + 1);
		});
	}

	[TestMethod]
	public async Task DbRepository_GetObjectAsync_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Assert
		await Assert.ThrowsExactlyAsync<Havit.Data.Patterns.Exceptions.ObjectNotFoundException>(async () =>
		{
			// Act
			await repository.GetObjectAsync(maxId + 1, TestContext.CancellationToken);
		});
	}

	[TestMethod]
	public void DbRepository_GetObject_ReturnsDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted repositoryResult = repository.GetObject(ids[0]);

		// Assert
		Assert.IsNotNull(repositoryResult);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectAsync_ReturnsDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted repositoryResult = await repository.GetObjectAsync(ids[0], TestContext.CancellationToken);

		// Assert
		Assert.IsNotNull(repositoryResult);
	}

	[TestMethod]
	public void DbRepository_GetAll_DoesNotReturnDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = repository.GetAll();

		// Assert
		Assert.IsEmpty(result);
	}

	[TestMethod]
	public void DbRepository_GetAll_ReturnsAllObjectAfterCommit()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result1 = repository.GetAll();
		Assert.IsEmpty(result1);

		testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
		testDbContext.SaveChanges();

		List<ItemWithDeleted> result2 = repository.GetAll();

		testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
		testDbContext.SaveChanges();

		List<ItemWithDeleted> result3 = repository.GetAll();

		// Assert
		Assert.HasCount(1, result2);
		Assert.HasCount(2, result3);
	}

	[TestMethod]
	public async Task DbRepository_GetAllAsync_ReturnsAllObjectAfterCommit()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result1 = await repository.GetAllAsync(TestContext.CancellationToken);
		Assert.IsEmpty(result1);

		testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
		await testDbContext.SaveChangesAsync(TestContext.CancellationToken);

		List<ItemWithDeleted> result2 = await repository.GetAllAsync(TestContext.CancellationToken);

		testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
		await testDbContext.SaveChangesAsync(TestContext.CancellationToken);

		List<ItemWithDeleted> result3 = await repository.GetAllAsync(TestContext.CancellationToken);

		// Assert
		Assert.HasCount(1, result2);
		Assert.HasCount(2, result3);
	}

	[TestMethod]
	public async Task DbRepository_GetAllAsync_DoesNotReturnDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = await repository.GetAllAsync(TestContext.CancellationToken);

		// Assert
		Assert.IsEmpty(result);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ReturnsObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = repository.GetObjects(ids);

		// Assert
		Assert.HasCount(ids.Length, result);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_ReturnsObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = await repository.GetObjectsAsync(ids, TestContext.CancellationToken);

		// Assert
		Assert.HasCount(ids.Length, result);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ReturnsDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		int[] ids = testDbContext.Set<ItemWithDeleted>()
			.Where(item => item.Deleted != null)
			.Select(item => item.Id)
			.ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = repository.GetObjects(ids);

		// Assert
		Assert.HasCount(ids.Length, result);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_ReturnsDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData(true);

		int[] ids = testDbContext.Set<ItemWithDeleted>()
			.Where(item => item.Deleted != null)
			.Select(item => item.Id)
			.ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> result = await repository.GetObjectsAsync(ids, TestContext.CancellationToken);

		// Assert
		Assert.HasCount(ids.Length, result);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Assert
		Assert.ThrowsExactly<Havit.Data.Patterns.Exceptions.ObjectNotFoundException>(() =>
		{
			// Act
			repository.GetObjects(maxId + 1, maxId + 2);
		});
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Assert
		await Assert.ThrowsExactlyAsync<Havit.Data.Patterns.Exceptions.ObjectNotFoundException>(async () =>
		{
			// Act
			await repository.GetObjectsAsync(new int[] { maxId + 1, maxId + 2 }, TestContext.CancellationToken);
		});
	}

	[TestMethod]
	public void DbRepository_GetObjects_AllowsDuplicateId()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> entities = repository.GetObjects(id, id, id); // duplicitní id (triplicitní)

		// Assert
		// no exception was thrown
		Assert.HasCount(1, entities);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_AllowsDuplicateId()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		List<ItemWithDeleted> entities = await repository.GetObjectsAsync(new int[] { id, id, id }, TestContext.CancellationToken); // duplicitní id (triplicitní)

		// Assert
		// no exception was thrown
		Assert.HasCount(1, entities);
	}

	[TestMethod]
	public void DbRepository_GetObject_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted entity = repository.GetObject(id);

		// Assert
		// no exception was thrown

		Assert.AreEqual(EntityState.Unchanged, testDbContext.Entry(entity).State);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectAsync_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted entity = await repository.GetObjectAsync(id, TestContext.CancellationToken);

		// Assert
		// no exception was thrown
		Assert.AreEqual(EntityState.Unchanged, testDbContext.Entry(entity).State);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted entity = repository.GetObjects(id).Single();

		// Assert
		// no exception was thrown
		Assert.AreEqual(EntityState.Unchanged, testDbContext.Entry(entity).State);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		SeedData();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new FakeDataLoader();
		var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, entityKeyAccessor, dataLoader, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), new FakeRepositoryQueryProvider());

		// Act
		ItemWithDeleted entity = (await repository.GetObjectsAsync(new int[] { id }, TestContext.CancellationToken)).Single();

		// Assert
		// no exception was thrown
		Assert.AreEqual(EntityState.Unchanged, testDbContext.Entry(entity).State);
	}

	private void SeedData(bool deleted = false, [CallerMemberName] string databaseName = default)
	{
		using (TestDbContext testDbContext = new TestDbContext(databaseName))
		{
			for (int i = 0; i < 3; i++)
			{
				ItemWithDeleted item = new ItemWithDeleted();
				if (deleted)
				{
					item.Deleted = DateTime.Now;
				}
				testDbContext.Set<ItemWithDeleted>().Add(item);
			}
			testDbContext.SaveChanges();
		}
	}

	private DbEntityKeyAccessor<TEntity, int> CreateEntityKeyAccessor<TEntity>(IDbContext dbContext)
		where TEntity : class
	{
		return new DbEntityKeyAccessor<TEntity, int>(new DbEntityKeyAccessor(new DbEntityKeyAccessorStorageBuilder(dbContext).Build()));

	}

	internal class FakeRepositoryQueryProvider : IRepositoryQueryProvider<ItemWithDeleted, int>
	{
		Func<DbContext, int, ItemWithDeleted> IRepositoryQueryProvider<ItemWithDeleted, int>.GetGetObjectQuery()
		{
			return EF.CompileQuery((DbContext dbContext, int id) => dbContext.Set<ItemWithDeleted>().Where(item => item.Id == id).FirstOrDefault());
		}

		Func<DbContext, int, CancellationToken, Task<ItemWithDeleted>> IRepositoryQueryProvider<ItemWithDeleted, int>.GetGetObjectAsyncQuery()
		{
			return EF.CompileAsyncQuery((DbContext dbContext, int id, CancellationToken cancellationToken) => dbContext.Set<ItemWithDeleted>().Where(item => item.Id == id).FirstOrDefault());
		}

		Func<DbContext, int[], IEnumerable<ItemWithDeleted>> IRepositoryQueryProvider<ItemWithDeleted, int>.GetGetObjectsQuery()
		{
			return EF.CompileQuery((DbContext dbContext, System.Int32[] ids) => dbContext.Set<ItemWithDeleted>().Where(entity => ids.Contains(entity.Id)));
		}

		Func<DbContext, int[], IAsyncEnumerable<ItemWithDeleted>> IRepositoryQueryProvider<ItemWithDeleted, int>.GetGetObjectsAsyncQuery()
		{
			return EF.CompileAsyncQuery((DbContext dbContext, System.Int32[] ids) => dbContext.Set<ItemWithDeleted>().Where(entity => ids.Contains(entity.Id)));
		}

	}
}
