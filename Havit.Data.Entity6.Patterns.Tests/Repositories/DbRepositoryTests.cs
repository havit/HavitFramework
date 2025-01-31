﻿using System.Data.Entity;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Patterns.Tests.Helpers;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.Repositories;

[TestClass]
public class DbRepositoryTests
{
	[ClassInitialize]
	public static void Initialize(TestContext testContext)
	{
		using (TestDbContext testDbContext = new TestDbContext())
		{
			testDbContext.Database.Initialize(true);
		}

		SeedData();

	}
	[ClassCleanup]
	public static void CleanUp()
	{
		DeleteDatabaseHelper.DeleteDatabase<TestDbContext>();
	}

	[TestMethod]
	[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException), AllowDerivedTypes = false)]
	public void DbRepository_GetObject_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		await repository.GetObjectAsync(maxId + 1);

		// Assert by method attribute 
	}
	[TestMethod]
	public void DbRepository_GetObject_ReturnsDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int[] ids = testDbContext.Set<ItemWithDeleted>().Where(item => item.Deleted != null).Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		List<ItemWithDeleted> result = repository.GetAll();

		// Assert
		Assert.IsTrue(result.Count > 0);
		Assert.IsTrue(result.All(item => item.Deleted == null));
	}

	[TestMethod]
	public async Task DbRepository_GetAllAsync_DoesNotReturnDeletedObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		List<ItemWithDeleted> result = await repository.GetAllAsync();

		// Assert
		Assert.IsTrue(result.Count > 0);
		Assert.IsTrue(result.All(item => item.Deleted == null));
	}

	[TestMethod]
	public void DbRepository_GetAll_ReturnsAllObjectAfterCommit()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		List<ItemWithDeleted> result1;
		List<ItemWithDeleted> result2;
		List<ItemWithDeleted> result3;

		// Act
		using (DbContextTransaction transaction = testDbContext.Database.BeginTransaction())
		{
			result1 = repository.GetAll();

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();

			result2 = repository.GetAll();

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();

			result3 = repository.GetAll();
			transaction.Rollback();
		}

		// Assert
		Assert.AreEqual(result1.Count + 1, result2.Count);
		Assert.AreEqual(result1.Count + 2, result3.Count);
	}

	[TestMethod]
	public async Task DbRepository_GetAllAsync_ReturnsAllObjectAfterCommit()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		List<ItemWithDeleted> result1;
		List<ItemWithDeleted> result2;
		List<ItemWithDeleted> result3;

		// Act
		using (DbContextTransaction transaction = testDbContext.Database.BeginTransaction())
		{
			result1 = await repository.GetAllAsync();

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			await testDbContext.SaveChangesAsync();

			result2 = await repository.GetAllAsync();

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			await testDbContext.SaveChangesAsync();

			result3 = await repository.GetAllAsync();
		}

		// Assert
		Assert.AreEqual(result1.Count + 1, result2.Count);
		Assert.AreEqual(result1.Count + 2, result3.Count);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ReturnsObjects()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int[] ids = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int[] ids = testDbContext.Set<ItemWithDeleted>()
			.Where(item => item.Deleted != null)
			.Select(item => item.Id)
			.ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int[] ids = testDbContext.Set<ItemWithDeleted>()
			.Where(item => item.Deleted != null)
			.Select(item => item.Id)
			.ToArray();
		Assert.AreNotEqual(0, ids.Length, "Pro test jsou potřeba data smazaná příznakem.");

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int maxId = testDbContext.Set<ItemWithDeleted>().Max(item => item.Id);

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		await repository.GetObjectsAsync(new int[] { maxId + 1, maxId + 2 });

		// Assert by method attribute
	}

	[TestMethod]
	public void DbRepository_GetObjects_AllowsDuplicateId()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

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

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		List<ItemWithDeleted> entities = await repository.GetObjectsAsync(new int[] { id, id, id }); // duplicitní id (triplicitní)

		// Assert
		// no exception was thrown
		Assert.AreEqual(1, entities.Count);
	}

	[TestMethod]
	public void DbRepository_GetObject_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		ItemWithDeleted entity = repository.GetObject(id);

		// Assert
		// no exception was thrown
		Assert.AreEqual(((IDbContext)testDbContext).GetEntityState(entity), EntityState.Unchanged);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectAsync_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		ItemWithDeleted entity = await repository.GetObjectAsync(id);

		// Assert
		// no exception was thrown
		Assert.AreEqual(((IDbContext)testDbContext).GetEntityState(entity), EntityState.Unchanged);
	}

	[TestMethod]
	public void DbRepository_GetObjects_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		ItemWithDeleted entity = repository.GetObjects(id).Single();

		// Assert
		// no exception was thrown
		Assert.AreEqual(((IDbContext)testDbContext).GetEntityState(entity), EntityState.Unchanged);
	}

	[TestMethod]
	public async Task DbRepository_GetObjectsAsync_ReturnTrackedObject()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		int id = testDbContext.Set<ItemWithDeleted>().Select(item => item.Id).First(); // načteme jen identifikátor, nikoliv objekt!

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act
		ItemWithDeleted entity = (await repository.GetObjectsAsync(new int[] { id })).Single();

		// Assert
		// no exception was thrown
		Assert.AreEqual(((IDbContext)testDbContext).GetEntityState(entity), EntityState.Unchanged);
	}

	/// <summary>
	/// Bug 24218: DbRepository: Po commitu (někdy) přestane fungovat GetObject
	/// </summary>
	[TestMethod]
	public void DbRepository_DbSetLocalsDictionary_IsReinitializedWithDataAfterSaveChanges()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();

		DbDataLoader dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());

		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));

		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, softDeleteManager);

		// Act
		ItemWithDeleted itemWithDeleted = testDbContext.Set<ItemWithDeleted>().First(); // načteme data do paměti jiným způsobem než přes repository

		testDbContext.SaveChanges(); // neobsahuje žádné změny

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

		var dataLoader = new DbDataLoader(testDbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		var dataSource = new DbItemWithDeletedDataSource(testDbContext, new SoftDeleteManager(new ServerTimeService()));
		DbItemWithDeletedRepository repository = new DbItemWithDeletedRepository(testDbContext, dataSource, dataLoader, new SoftDeleteManager(new ServerTimeService()));

		// Act + Assert
		using (DbContextTransaction transaction = testDbContext.Database.BeginTransaction())
		{
			var items = repository.GetAll(); // načteme objekty do identity mapy (DbSet<>.Locals).

			Assert.IsTrue(items.Count > 0); // Prerequisite

			Assert.AreEqual(items.Count, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary odpovídá počtu načtených prvků (viz předchozí GetAll).

			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());

			Assert.AreEqual(items.Count, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary se přidáním nového prvku nezměnil (před uložením)

			testDbContext.SaveChanges();

			Assert.AreEqual(items.Count + 1, repository.DbSetLocalsDictionary.Count); // počet prvků v dictionary se po uložení nového objektu zvýšil o jeden

			transaction.Rollback();
		}
	}

	private static void SeedData()
	{
		using (TestDbContext testDbContext = new TestDbContext())
		{
			for (int i = 0; i < 10; i++)
			{
				ItemWithDeleted item = new ItemWithDeleted();
				item.Deleted = item.Deleted = i >= 5 ? DateTime.Now : null;
				testDbContext.Set<ItemWithDeleted>().Add(item);
			}
			testDbContext.SaveChanges();
		}
	}
}
