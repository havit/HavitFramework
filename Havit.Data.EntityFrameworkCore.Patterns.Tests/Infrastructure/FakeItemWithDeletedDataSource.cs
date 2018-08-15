using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	public class FakeItemWithDeletedDataSource : FakeDataSource<ItemWithDeleted>
	{
		public FakeItemWithDeletedDataSource(params ItemWithDeleted[] data)
			: base(data)
		{
		}
	}
}
