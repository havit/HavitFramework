using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.QueryServices;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.QueryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.QueryServices
{
	[TestClass]
	public class DbQueryAllServiceTest
	{
		[TestMethod]
		public void DbQueryAllService_GetAll_ReturnsAllNotDeletedObjects()
		{
			// Arrange
			ItemWithDeleted[] definedItems = new ItemWithDeleted[] { new ItemWithDeleted(), new ItemWithDeleted(), new ItemWithDeleted() };
			IDataSource<ItemWithDeleted> fakeDataSource = new FakeItemWithDeletedDataSource(definedItems);
			DbQueryAllService<ItemWithDeleted> dbQueryAllService = new DbQueryAllService<ItemWithDeleted>(fakeDataSource);
			
			// Act
			List<ItemWithDeleted> items = dbQueryAllService.QueryAll();

			// Assert
			CollectionAssert.AreEqual(definedItems, items);
		}

		[TestMethod]
		public void DbQueryAllService_GetAll_DoNotReturnDeletedObjects()
		{
			// Arrange
			ItemWithDeleted definedDeletedItem = new ItemWithDeleted { Deleted = DateTime.Now };
			IDataSource<ItemWithDeleted> fakeDataSource = new FakeItemWithDeletedDataSource(definedDeletedItem);
			DbQueryAllService<ItemWithDeleted> dbQueryAllService = new DbQueryAllService<ItemWithDeleted>(fakeDataSource);

			// Act
			List<ItemWithDeleted> items = dbQueryAllService.QueryAll();

			// Assert
			Assert.AreEqual(0, items.Count);
		}

		[TestMethod]
		public async Task DbQueryAllService_GetAllAsync_ReturnsAllObject()
		{
			// Arrange
			ItemWithDeleted[] definedItems = { new ItemWithDeleted(), new ItemWithDeleted(), new ItemWithDeleted() };
			IDataSource<ItemWithDeleted> fakeDataSource = new FakeItemWithDeletedDataSource(definedItems);
			DbQueryAllService<ItemWithDeleted> dbQueryAllService = new DbQueryAllService<ItemWithDeleted>(fakeDataSource);

			// Act
			List<ItemWithDeleted> items = await dbQueryAllService.QueryAllAsync();

			// Assert
			CollectionAssert.AreEqual(definedItems, items);
		}
	}
}
