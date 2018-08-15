using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	public class FakeInt32DataSource : FakeDataSource<int>
	{
		public FakeInt32DataSource(params int[] data) : base(data)
		{
		}
	}
}
