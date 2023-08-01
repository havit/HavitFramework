using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSources.Fakes;

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
	public async Task FakeDataSource_Async_WithCondition()
	{
		// Arrange
		int[] sourceData = Enumerable.Range(1, 100).ToArray();
		FakeDataSource<int> dataSource = new FakeInt32DataSource(sourceData);

		//Act
		List<int> resultData = await dataSource.Data.Skip(5).Where(i => i > 3).Skip(1).Take(3).ToListAsync();

		// Assert
		CollectionAssert.AreEqual(new List<int> { 7, 8, 9 }, resultData);
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
