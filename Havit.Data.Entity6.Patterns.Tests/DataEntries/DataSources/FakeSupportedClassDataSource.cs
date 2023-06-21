using Havit.Data.Entity.Patterns.DataSources.Fakes;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources;

public class FakeSupportedClassDataSource : FakeDataSource<SupportedClass>
{
	public FakeSupportedClassDataSource(params SupportedClass[] data)
		: base(data)
	{
	}
	
}
