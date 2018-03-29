using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class DbItemWithDeletedRepository : DbRepository<ItemWithDeleted>
	{
		public DbItemWithDeletedRepository(IDbContext dbContext, IDataSource<ItemWithDeleted> dataSource, IEntityKeyAccessor<ItemWithDeleted, int> entityKeyAccessor, IDataLoader dataLoader, IDataLoaderAsync dataLoaderAsync, ISoftDeleteManager softDeleteManager)
			: base(dbContext, dataSource, entityKeyAccessor, dataLoader, dataLoaderAsync, softDeleteManager)
		{
		}
	}
}