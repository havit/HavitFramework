using Havit.Data.Entity.Patterns.QueryServices.Fakes;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class FakeLanguageDataSource : FakeDataSource<Language>
	{
		public FakeLanguageDataSource(params Language[] data) : base(data)
		{
		}
	}
}
