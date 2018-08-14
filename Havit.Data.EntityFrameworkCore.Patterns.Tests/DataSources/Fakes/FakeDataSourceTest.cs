using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataSources.Fakes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.DataSources.Fakes
{
	[TestClass]
	public class FakeDataSourceTest
	{
		[TestMethod]
		public void FakeDataSource_SupportsEmptyData()
		{
			// Arrange
			int[] sourceData = Enumerable.Empty<int>().ToArray();
			FakeDataSource<int> dataSource = new FakeInt32DataSource(sourceData);

			//Act
			List<int> resultData = dataSource.Data.ToList();

			// Assert
			Assert.AreEqual(0, resultData.Count);
		}
		
		[TestMethod]
		public void FakeDataSource_ReturnsSourceData()
		{
			// Arrange
			int[] sourceData = Enumerable.Range(1, 5).ToArray();
			FakeDataSource<int> dataSource = new FakeInt32DataSource(sourceData);			

			//Act
			List<int> resultData = dataSource.Data.ToList();

			// Assert
			CollectionAssert.AreEqual(sourceData, resultData);
		}

		[TestMethod]
		public async Task FakeDataSource_Async_ReturnsSourceData()
		{
			// Arrange
			int[] sourceData = Enumerable.Range(1, 5).ToArray();
			FakeDataSource<int> dataSource = new FakeInt32DataSource(sourceData);

			//Act
			List<int> resultData = await dataSource.Data.ToListAsync();

			// Assert
			CollectionAssert.AreEqual(sourceData, resultData);
		}

		[TestMethod]
		public void FakeDataSource_DoNotReturnDeletedObjects()
		{
			// Arrange
			FakeDataSource<ItemWithDeleted> dataSource = new FakeItemWithDeletedDataSource(new ItemWithDeleted { Deleted = DateTime.Now });

			//Act
			List<ItemWithDeleted> resultData = dataSource.Data.ToList();

			// Assert
			Assert.AreEqual(0, resultData.Count);
		}

		[TestMethod]
		public async Task FakeDataSource_Async_DoNotReturnDeletedObjects()
		{
			// Arrange
			FakeDataSource<ItemWithDeleted> dataSource = new FakeItemWithDeletedDataSource(new ItemWithDeleted { Deleted = DateTime.Now });

			//Act
			List<ItemWithDeleted> resultData = await dataSource.Data.ToListAsync();

			// Assert
			Assert.AreEqual(0, resultData.Count);
		}

	}
}
