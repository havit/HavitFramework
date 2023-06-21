using Havit.Data.Entity.Patterns.DataSources.Fakes;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class FakeInt32DataSource : FakeDataSource<int>
{
	public FakeInt32DataSource(params int[] data) : base(data)
	{
	}
}
