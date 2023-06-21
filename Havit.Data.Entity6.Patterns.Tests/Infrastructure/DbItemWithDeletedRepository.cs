using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSources;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class DbItemWithDeletedRepository : DbRepository<ItemWithDeleted>
{
	public DbItemWithDeletedRepository(IDbContext dbContext, IDataSource<ItemWithDeleted> dataSource, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager)
		: base(dbContext, dataSource, dataLoader, softDeleteManager)
	{
	}
}