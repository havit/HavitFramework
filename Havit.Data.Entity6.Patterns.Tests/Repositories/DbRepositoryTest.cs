using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Data.Patterns.DataLoaders;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.Repositories
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
			testDbContext.Database.Initialize(true);

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			await repository.GetObjectAsync(maxId + 1);

			// Assert by method attribute 
		}
		[TestMethod]
		public void DbRepository_GetObject_ReturnsDeletedObjects()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			SeedData(true);

			int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData(true);

			int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData(true);

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = repository.GetAll();

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task DbRepository_GetAllAsync_DoesNotReturnDeletedObjects()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			SeedData(true);

			var dataLoader = new DbDataLoader(testDbContext);
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData();

			int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));
			
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
			testDbContext.Database.Initialize(true);

			SeedData();

			int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData(true);

			int[] ids = testDbContext.Set<ItemWithDeleted>()
				.Where(item => item.Deleted != null)
				.Select(item => item.Id)
				.ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData(true);

			int[] ids = testDbContext.Set<ItemWithDeleted>()
				.Where(item => item.Deleted != null)
				.Select(item => item.Id)
				.ToArray();
			Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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
			testDbContext.Database.Initialize(true);

			SeedData();

			int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act
			await repository.GetObjectsAsync(maxId + 1, maxId + 2);

			// Assert by method attribute
		}

		/// <summary>
		/// Bug 24218: DbRepository: Po commitu (někdy) přestane fungovat GetObject
		/// </summary>
		[TestMethod]
		public void DbRepository_DbSetLocalsDictionary_IsReinitializedWithDataAfterSaveChanges()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);
			SeedData();

			DbDataLoader dataLoader = new DbDataLoader(testDbContext);
			SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, softDeleteManager);

			// Act
			ItemWithDeleted itemWithDeleted = testDbContext.Set<ItemWithDeleted>().First(); // načteme data do paměti jiným způsobem než přes repository

			testDbContext.SaveChanges();
			// po uložení změn dojde k reinicializaci dictionary, ovšem až si na něj šahneme a uložíme do proměné, protože je vyměněna celá instance dictionary
			Dictionary<int, ItemWithDeleted> dbSetLocalsDictionary = repository.DbSetLocalsDictionary; // (bug 24218 - zde aplikace spadla)
			
			// Assert
			Assert.IsTrue(dbSetLocalsDictionary.ContainsKey(itemWithDeleted.Id)); // ověříme, že se ID dostalo do dictionary
		}

		[TestMethod]
		public void DbRepository_DbSetLocalsDictionary_DoesNotContainNewObjects()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			SeedData();

			var dataLoader = new DbDataLoader(testDbContext);
			DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataLoader, dataLoader, new SoftDeleteManager(new ServerTimeService()));

			// Act + Assert
			var items = repository.GetAll(); // načteme objekty do identity mapy (DbSet<>.Locals).

			Assert.IsTrue(items.Count > 0); // Prerequisite

			Assert.AreEqual(items.Count, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary odpovídá počtu načtených prvků (viz předchozí GetAll).

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());

			Assert.AreEqual(items.Count, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary se přidáním nového prvku nezměnil (před uložením)

			testDbContext.SaveChanges();

			Assert.AreEqual(items.Count + 1, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary se po uložení nového objektu zvýšil o jeden
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
	}
}
