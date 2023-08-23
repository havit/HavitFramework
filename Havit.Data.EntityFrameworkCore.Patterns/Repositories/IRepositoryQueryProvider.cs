using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Poskytuje dotazy pro použití v repository.
/// </summary>
public interface IRepositoryQueryProvider
{
	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetObject(int)" />.
	/// </summary>
	Func<DbContext, int, TEntity> GetGetObjectCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjectAsync(int, CancellationToken)" />.
	/// </summary>
	Func<DbContext, int, CancellationToken, Task<TEntity>> GetGetObjectAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjects(int[])" />.
	/// </summary>
	Func<DbContext, int[], IEnumerable<TEntity>> GetGetObjectsCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetObjectsAsync(int[], CancellationToken)" />.
	/// </summary>
	Func<DbContext, int[], IAsyncEnumerable<TEntity>> GetGetObjectsAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor) where TEntity : class;

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetAll" />.
	/// </summary>
	Func<DbContext, IEnumerable<TEntity>> GetGetAllCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager) where TEntity : class;

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity}.GetAllAsync(CancellationToken)" />.
	/// </summary>
	Func<DbContext, IAsyncEnumerable<TEntity>> GetGetAllAsyncCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager) where TEntity : class;
}