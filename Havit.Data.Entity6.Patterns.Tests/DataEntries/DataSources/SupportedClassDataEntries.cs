using Havit.Data.Entity.Patterns.DataEntries;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources
{
	public class SupportedClassDataEntries : DbDataEntries<SupportedClass>
	{
		public SupportedClassDataEntries(IDataEntrySymbolStorage<SupportedClass> dataEntrySymbolStorage, IRepository<SupportedClass> repository) : base(dataEntrySymbolStorage, repository)
		{
		}

		public SupportedClass First
		{
			get { return GetEntry(SupportedClass.Entry.First); }
		}

		public SupportedClass Second
		{
			get { return GetEntry(SupportedClass.Entry.Second); }
		}

		public SupportedClass Third
		{
			get { return GetEntry(SupportedClass.Entry.Third); }
		}
	}
}