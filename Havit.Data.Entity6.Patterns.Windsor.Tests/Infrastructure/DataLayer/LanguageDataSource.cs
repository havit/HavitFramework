using Havit.Data.Entity;
using Havit.Data.Entity.Patterns.DataSources;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer;

public class LanguageDataSource : DbDataSource<Language>, ILanguageDataSource
{
	public LanguageDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
	{
	}
}
