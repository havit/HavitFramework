using Havit.Data.Entity.Patterns.DataSources.Fakes;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class FakeItemWithDeletedDataSource : FakeDataSource<ItemWithDeleted>
{
	public FakeItemWithDeletedDataSource(params ItemWithDeleted[] data)
		: base(data)
	{
	}
}
