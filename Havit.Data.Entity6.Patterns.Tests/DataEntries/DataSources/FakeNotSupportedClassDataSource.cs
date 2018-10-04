using Havit.Data.Entity.Patterns.DataSources.Fakes;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources
{
	public class FakeNotSupportedClassDataSource : FakeDataSource<NotSupportedClass>
	{
		public FakeNotSupportedClassDataSource(params NotSupportedClass[] data)
			: base(data)
		{
		}
		
	}
}
