using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Sestavuje dotazy pro použití v repository.
/// </summary>
public interface IRepositoryQueryBuilder
{
	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetObject(int)" />.
	/// </summary>
	Func<DbContext, int, TEntity> CreateGetObjectCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjectAsync(int, CancellationToken)" />.
	/// </summary>
	Func<DbContext, int, CancellationToken, Task<TEntity>> CreateGetObjectAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjects(int[])" />.
	/// </summary>
	Func<DbContext, int[], IEnumerable<TEntity>> CreateGetObjectsCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjectsAsync(int[], CancellationToken)" />.
	/// </summary>
	Func<DbContext, int[], IAsyncEnumerable<TEntity>> CreateGetObjectsAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetAll" />.
	/// </summary>
	Func<DbContext, IEnumerable<TEntity>> CreateGetAllCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager) where TEntity : class;

	/// <summary>
	/// Vytvoří dotaz pro metodu <see cref="DbRepository{TEntity}.GetAllAsync" />.
	/// </summary>
	Func<DbContext, IAsyncEnumerable<TEntity>> CreateGetAllAsyncCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager) where TEntity : class;

}