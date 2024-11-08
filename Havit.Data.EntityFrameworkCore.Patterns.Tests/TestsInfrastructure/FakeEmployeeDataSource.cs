using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class FakeEmployeeDataSource : FakeDataSource<Employee>
{
	public FakeEmployeeDataSource(params Employee[] data)
		: base(data)
	{
	}
}
