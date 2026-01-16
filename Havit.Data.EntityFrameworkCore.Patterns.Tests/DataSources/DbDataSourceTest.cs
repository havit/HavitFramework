using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSources;

[TestClass]
public class DbDataSourceTest
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void DbDataSource_DataWithDeleted_IncludesDeleted()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		List<ItemWithDeleted> result = dataSource.DataIncludingDeleted.ToList();

		// Assert
		Assert.HasCount(2, result);
	}

	[TestMethod]
	public void DbDataSource_Data_ExcludesDeleted()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		List<ItemWithDeleted> result = dataSource.Data.ToList();

		// Assert
		Assert.HasCount(1, result);
	}

	[TestMethod]
	public async Task DbDataSource_DataList_ExcludesDeleted()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		List<ItemWithDeleted> result = await dataSource.Data.ToListAsync(TestContext.CancellationToken);

		// Assert
		Assert.HasCount(1, result);
	}

	[TestMethod]
	public void DbDataSource_DataCount_ExcludesDeleted()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		int result = dataSource.Data.Count();

		// Assert
		Assert.AreEqual(1, result);
	}

	[TestMethod]
	public async Task DbDataSource_DataCountAsync_ExcludesDeleted()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		int result = await dataSource.Data.CountAsync(TestContext.CancellationToken);

		// Assert
		Assert.AreEqual(1, result);
	}

	[TestMethod]
	public void DbDataSource_IsReusable()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		int count1 = dataSource.Data.Count();
		int count2 = dataSource.Data.Count();
		int count3 = dataSource.Data.Count();
		int count4 = dataSource.DataIncludingDeleted.Count();
		int count5 = dataSource.DataIncludingDeleted.Count();

		// Assert
		Assert.AreEqual(1, count1);
		Assert.AreEqual(1, count2);
		Assert.AreEqual(1, count3);
		Assert.AreEqual(2, count4);
		Assert.AreEqual(2, count5);
	}

	[TestMethod]
	public void DbDataSource_DeletedObjectAreNotLoaded()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
		ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

		dbContext.Set<ItemWithDeleted>().Add(deletedItem);
		dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

		dbContext.SaveChanges();

		// Act
		dbContext = new TestDbContext(); // nový context pro novou identity map (a dbSets.Local).
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		int count = dataSource.Data.Count();
		List<ItemWithDeleted> items = dataSource.Data.ToList();

		// Assert
		Assert.AreEqual(1, count);
		Assert.HasCount(1, items);
		Assert.AreEqual(1, dbContext.Set<ItemWithDeleted>().Local.Count);
	}

	[TestMethod]
	public async Task DbDataSource_SupportsToListAsync()
	{
		// Arrange
		TestDbContext dbContext = new TestDbContext();
		dbContext.Database.DropCreate();

		// Act
		DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
		await dataSource.Data.ToListAsync(TestContext.CancellationToken);

		// Assert
		// not throwing exception is enough
	}
}
