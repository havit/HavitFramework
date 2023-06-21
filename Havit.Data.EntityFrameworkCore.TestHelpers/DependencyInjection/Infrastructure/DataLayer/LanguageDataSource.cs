using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;

public class LanguageDataSource : DbDataSource<Language>, ILanguageDataSource
{
	public LanguageDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
	{
	}
}
