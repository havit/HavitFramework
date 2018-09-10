using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.DataLoaders.Fakes;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Repositories
{
	[TestClass]
	public class DbRepositoryTest
	{
		[TestMethod]
		[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException), AllowDerivedTypes = false)]
		public void DbRepository_GetObject_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			repository.GetObject(maxId + 1);

			// Assert by method attribute 
		}

		[TestMethod]
		[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException), AllowDerivedTypes = false)]
		public async Task DbRepository_GetObjectAsync_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			await repository.GetObjectAsync(maxId + 1);

			// Assert by method attribute 
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			ItemWithDeleted repositoryResult = await repository.GetObjectAsync(ids[0]);

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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = repository.GetAll();

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void DbRepository_GetAll_ReturnsAllObjectAfterCommit()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result1 = repository.GetAll();
			Assert.AreEqual(0, result1.Count);

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();

			List<ItemWithDeleted> result2 = repository.GetAll();

			// Assert
			Assert.AreEqual(1, result2.Count);
		}

		[TestMethod]
		public async Task  DbRepository_GetAllAsync_ReturnsAllObjectAfterCommit()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result1 = await repository.GetAllAsync();
			Assert.AreEqual(0, result1.Count);
			
			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();

			List<ItemWithDeleted> result2 = await repository.GetAllAsync();

			// Assert
			Assert.AreEqual(1, result2.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = await repository.GetAllAsync();

			// Assert
			Assert.AreEqual(0, result.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));
			
			// Act
			List<ItemWithDeleted> result = repository.GetObjects(ids);

			// Assert
			Assert.AreEqual(ids.Length, result.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = await repository.GetObjectsAsync(ids);

			// Assert
			Assert.AreEqual(ids.Length, result.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = repository.GetObjects(ids);

			// Assert
			Assert.AreEqual(ids.Length, result.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = await repository.GetObjectsAsync(ids);

			// Assert
			Assert.AreEqual(ids.Length, result.Count);
		}

		[TestMethod]
		[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException), AllowDerivedTypes = false)]
		public void DbRepository_GetObjects_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			repository.GetObjects(maxId + 1, maxId + 2);

			// Assert by method attribute
		}

		[TestMethod]
		[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException), AllowDerivedTypes = false)]
		public async Task DbRepository_GetObjectsAsync_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.DropCreate();

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new FakeDataLoader();
			var entityKeyAccessor = CreateEntityKeyAccessor<ItemWithDeleted>(testDbContext);
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			await repository.GetObjectsAsync(maxId + 1, maxId + 2);

			// Assert by method attribute
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> entities = repository.GetObjects(id, id, id); // duplicitní id (triplicitní)

			// Assert
			// no exception was thrown
			Assert.AreEqual(1, entities.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> entities = await repository.GetObjectsAsync(id, id, id); // duplicitní id (triplicitní)

			// Assert
			// no exception was thrown
			Assert.AreEqual(1, entities.Count);
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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			ItemWithDeleted entity = await repository.GetObjectAsync(id);

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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, entityKeyAccessor, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			ItemWithDeleted entity = (await repository.GetObjectsAsync(id)).Single(); // duplicitní id (triplicitní)

			// Assert
			// no exception was thrown
			Assert.AreEqual(EntityState.Unchanged, testDbContext.Entry(entity).State);
		}

		private void SeedData(bool deleted = false)
		{
			using (TestDbContext testDbContext = new TestDbContext())
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
			var testDbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			testDbContextFactoryMock.Setup(m => m.CreateService()).Returns(dbContext);
			testDbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));
			return new DbEntityKeyAccessor<TEntity, int>(testDbContextFactoryMock.Object);

		}
	}
}
