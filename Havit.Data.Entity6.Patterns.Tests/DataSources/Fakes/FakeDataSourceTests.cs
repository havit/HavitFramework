using Havit.Data.Entity.Patterns.DataSources.Fakes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using System.Data.Entity;

namespace Havit.Data.Entity.Patterns.Tests.DataSources.Fakes;

[TestClass]
public class FakeDataSourceTests
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
		Assert.IsEmpty(resultData);
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
		Assert.IsEmpty(resultData);
	}

	[TestMethod]
	public async Task FakeDataSource_Async_DoNotReturnDeletedObjects()
	{
		// Arrange
		FakeDataSource<ItemWithDeleted> dataSource = new FakeItemWithDeletedDataSource(new ItemWithDeleted { Deleted = DateTime.Now });

		//Act
		List<ItemWithDeleted> resultData = await dataSource.Data.ToListAsync();

		// Assert
		Assert.IsEmpty(resultData);
	}

}
