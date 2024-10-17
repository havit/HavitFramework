using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <inheritdoc />
public class RepositoryQueryProvider : IRepositoryQueryProvider
{
	private readonly IRepositoryQueryStore repositoryCompiledQueryStore;
	private readonly IRepositoryQueryBuilder repositoryCompiledQueryBuilder;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public RepositoryQueryProvider(IRepositoryQueryStore repositoryCompiledQueryStore, IRepositoryQueryBuilder repositoryCompiledQueryBuilder)
	{
		this.repositoryCompiledQueryStore = repositoryCompiledQueryStore;
		this.repositoryCompiledQueryBuilder = repositoryCompiledQueryBuilder;
	}

	/// <inheritdoc />
	public Func<DbContext, int, TEntity> GetGetObjectCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int, TEntity>)repositoryCompiledQueryStore.GetObjectCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetObjectCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	/// <inheritdoc />
	public Func<DbContext, int, CancellationToken, Task<TEntity>> GetGetObjectAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int, CancellationToken, Task<TEntity>>)repositoryCompiledQueryStore.GetObjectAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetObjectAsyncCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	/// <inheritdoc />
	public Func<DbContext, int[], IEnumerable<TEntity>> GetGetObjectsCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int[], IEnumerable<TEntity>>)repositoryCompiledQueryStore.GetObjectsCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetObjectsCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	/// <inheritdoc />
	public Func<DbContext, int[], IAsyncEnumerable<TEntity>> GetGetObjectsAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int[], IAsyncEnumerable<TEntity>>)repositoryCompiledQueryStore.GetObjectsAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetObjectsAsyncCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	/// <inheritdoc />
	public Func<DbContext, IEnumerable<TEntity>> GetGetAllCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return (Func<DbContext, IEnumerable<TEntity>>)repositoryCompiledQueryStore.GetAllCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetAllCompiledQuery<TEntity>(repositoryType, softDeleteManager));
	}

	/// <inheritdoc />
	public Func<DbContext, IAsyncEnumerable<TEntity>> GetGetAllAsyncCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return (Func<DbContext, IAsyncEnumerable<TEntity>>)repositoryCompiledQueryStore.GetAllAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => repositoryCompiledQueryBuilder.CreateGetAllAsyncCompiledQuery<TEntity>(repositoryType, softDeleteManager));
	}
}
