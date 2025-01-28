using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class DbItemWithDeletedRepository : DbRepository<ItemWithDeleted>
{
	public DbItemWithDeletedRepository(IDbContext dbContext, IEntityKeyAccessor<ItemWithDeleted, int> entityKeyAccessor, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IRepositoryQueryProvider<ItemWithDeleted, int> repositoryCompiledQueryProvider)
		: base(dbContext, entityKeyAccessor, dataLoader, softDeleteManager, entityCacheManager, repositoryCompiledQueryProvider)
	{
	}
}