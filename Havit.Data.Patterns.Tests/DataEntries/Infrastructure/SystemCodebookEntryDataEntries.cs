using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.Patterns.Tests.DataEntries.Infrastructure;

public class SystemCodebookEntryDataEntries : DataEntries<SystemCodebookEntry>
{
	public SystemCodebookEntryDataEntries(IDataEntrySymbolService<SystemCodebookEntry> dataEntrySymbolService, IRepository<SystemCodebookEntry> repository) : base(dataEntrySymbolService, repository)
	{
	}

	public SystemCodebookEntryDataEntries(IRepository<SystemCodebookEntry> repository) : base(repository)
	{
	}

	public SystemCodebookEntry First => GetEntry(SystemCodebookEntry.Entry.First);

	public SystemCodebookEntry Second => GetEntry(SystemCodebookEntry.Entry.Second);
}