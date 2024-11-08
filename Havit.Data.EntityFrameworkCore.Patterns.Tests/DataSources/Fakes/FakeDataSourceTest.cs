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
		Employee[] sourceData = Enumerable.Empty<Employee>().ToArray();
		var dataSource = new FakeEmployeeDataSource(sourceData);

		//Act
		List<Employee> resultData = dataSource.Data.ToList();

		// Assert
		Assert.AreEqual(0, resultData.Count);
	}

	[TestMethod]
	public void FakeDataSource_ReturnsSourceData()
	{
		// Arrange
		Employee[] sourceData = Enumerable.Range(1, 5).Select(i => new Employee { Id = i }).ToArray();
		var dataSource = new FakeEmployeeDataSource(sourceData);

		//Act
		List<Employee> resultData = dataSource.Data.ToList();

		// Assert
		CollectionAssert.AreEqual(sourceData, resultData);
	}

	[TestMethod]
	public async Task FakeDataSource_Async_ReturnsSourceData()
	{
		// Arrange
		Employee[] sourceData = Enumerable.Range(1, 5).Select(i => new Employee { Id = i }).ToArray();
		var dataSource = new FakeEmployeeDataSource(sourceData);

		//Act
		List<Employee> resultData = await dataSource.Data.ToListAsync();

		// Assert
		CollectionAssert.AreEqual(sourceData, resultData);
	}

	[TestMethod]
	public async Task FakeDataSource_Async_WithCondition()
	{
		// Arrange
		Employee[] sourceData = Enumerable.Range(1, 100).Select(i => new Employee { Id = i }).ToArray();
		var dataSource = new FakeEmployeeDataSource(sourceData);

		//Act
		Employee result = await dataSource.Data
			.Include(e => e.Boss)
			.ThenInclude(e => e.Subordinates)
			.Skip(5)
			.Where(item => item.Id > 3)
			.Skip(1)
			.Take(3)
			.FirstOrDefaultAsync();

		// Assert
		Assert.AreEqual(7, result.Id);
	}

	[TestMethod]
	public void FakeDataSource_DoNotReturnDeletedObjects()
	{
		// Arrange
		var dataSource = new FakeEmployeeDataSource(new Employee { Deleted = DateTime.Now });

		//Act
		List<Employee> resultData = dataSource.Data.ToList();

		// Assert
		Assert.AreEqual(0, resultData.Count);
	}

	[TestMethod]
	public async Task FakeDataSource_Async_DoNotReturnDeletedObjects()
	{
		// Arrange
		var dataSource = new FakeEmployeeDataSource(new Employee { Deleted = DateTime.Now });

		//Act
		List<Employee> resultData = await dataSource.Data.ToListAsync();

		// Assert
		Assert.AreEqual(0, resultData.Count);
	}

}
