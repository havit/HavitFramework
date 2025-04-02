using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer;

public class LanguageEntries : DataEntries<Language, System.Int32>, ILanguageEntries
{
	public LanguageEntries(IDataEntrySymbolService<Language, System.Int32> dataEntrySymbolService, IRepository<Language, System.Int32> repository) : base(dataEntrySymbolService, repository)
	{
	}
}
