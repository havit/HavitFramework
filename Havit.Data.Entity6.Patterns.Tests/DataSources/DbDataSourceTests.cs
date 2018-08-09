using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataSources;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Helpers;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.DataSources
{
	[TestClass]
	public class DbDataSourceTests
	{
		[ClassCleanup]
		public static void CleanUp()
		{
			DeleteDatabaseHelper.DeleteDatabase<TestDbContext>();
		}

		[TestMethod]
		public void DbDataSource_DataWithDeleted_IncludesDeleted()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

			ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
			ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

			dbContext.Set<ItemWithDeleted>().Add(deletedItem);
			dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

			dbContext.SaveChanges();

			// Act
			DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			List<ItemWithDeleted> result = dataSource.DataWithDeleted.ToList();			

			// Assert
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public void DbDataSource_Data_ExcludesDeleted()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

			ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
			ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

			dbContext.Set<ItemWithDeleted>().Add(deletedItem);
			dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

			dbContext.SaveChanges();

			// Act
			DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			List<ItemWithDeleted> result = dataSource.Data.ToList();			

			// Assert
			Assert.AreEqual(1, result.Count);
		}

		[TestMethod]
		public async Task DbDataSource_DataList_ExcludesDeleted()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

			ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
			ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

			dbContext.Set<ItemWithDeleted>().Add(deletedItem);
			dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

			dbContext.SaveChanges();

			// Act
			DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			List<ItemWithDeleted> result = await dataSource.Data.ToListAsync();

			// Assert
			Assert.AreEqual(1, result.Count);
		}

		[TestMethod]
		public void DbDataSource_DataCount_ExcludesDeleted()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

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
			dbContext.Database.Initialize(true);

			ItemWithDeleted deletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
			ItemWithDeleted notDeletedItem = new ItemWithDeleted { Deleted = null };

			dbContext.Set<ItemWithDeleted>().Add(deletedItem);
			dbContext.Set<ItemWithDeleted>().Add(notDeletedItem);

			dbContext.SaveChanges();

			// Act
			DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			int result = await dataSource.Data.CountAsync();

			// Assert
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void DbDataSource_IsReusable()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

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
			int count4 = dataSource.DataWithDeleted.Count();
			int count5 = dataSource.DataWithDeleted.Count();

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
			dbContext.Database.Initialize(true);

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
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, dbContext.Set<ItemWithDeleted>().Local.Count);
		}

		[TestMethod]
		public async Task DbDataSource_SupportsToListAsync()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			dbContext.Database.Initialize(true);

			// Act
			DbDataSource<ItemWithDeleted> dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			await dataSource.Data.ToListAsync();

			// Assert
			// not throwing exception is enough
		}
	}
}
