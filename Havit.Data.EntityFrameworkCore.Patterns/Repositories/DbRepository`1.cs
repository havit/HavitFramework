using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Repository objektů typu TEntity s primárním klíčem typu System.Int32.
/// </summary>
/// <remarks>
/// Pro zpětnou kompatilitu tam, kde je potřeba aby repository implementovala <see cref="IRepository{TEntity}"/> (a nejen <see cref="IRepository{TEntity, TKey}"/>.
/// </remarks>
public abstract class DbRepository<TEntity> : DbRepository<TEntity, int>, IRepository<TEntity>
	 where TEntity : class
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected DbRepository(IDbContext dbContext, IEntityKeyAccessor<TEntity, int> entityKeyAccessor, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IRepositoryQueryProvider<TEntity, int> repositoryQueryProvider)
		: base(dbContext, entityKeyAccessor, dataLoader, softDeleteManager, entityCacheManager, repositoryQueryProvider)
	{
		// NOOP
	}
}