using Havit.Data.Entity.Patterns.DataSources;
using Havit.Data.Entity.Patterns.SoftDeletes;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class DbItemWithDeletedDataSource : DbDataSource<ItemWithDeleted>
	{
		public DbItemWithDeletedDataSource(IDbContext dbContext, SoftDeleteManager softDeleteManager) : base(dbContext, softDeleteManager)
		{
		}
	}
}