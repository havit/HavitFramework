using Havit.Data.Entity.Patterns.DataSources.Fakes;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class FakeLanguageDataSource : FakeDataSource<Language>
	{
		public FakeLanguageDataSource(params Language[] data) : base(data)
		{
		}
	}
}
