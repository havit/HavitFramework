using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class DbItemWithDeletedRepository : DbRepository<ItemWithDeleted>
	{
		public DbItemWithDeletedRepository(IDbContext dbContext, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager)
			: base(dbContext, dataLoader, dataLoaderAsync, softDeleteManager)
		{
		}
	}
}