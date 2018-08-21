using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer
{
	public class LanguageDataSource : DbDataSource<Language>
	{
		public LanguageDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
		{
		}
	}
}
