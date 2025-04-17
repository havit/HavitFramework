namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Poskytuje dotazy pro použití v repository.
/// </summary>
/// <remarks>
/// API je šité na míru pro kompilované dotazy EF Core.
/// </remarks>
public interface IRepositoryQueryProvider<TEntity, TKey>
	where TEntity : class
{
	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity, TKey}.GetObject(TKey)" />.
	/// </summary>
	Func<DbContext, TKey, TEntity> GetGetObjectQuery();

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity, TKey}.GetObjectAsync(TKey, CancellationToken)" />.
	/// </summary>
	Func<DbContext, TKey, CancellationToken, Task<TEntity>> GetGetObjectAsyncQuery();

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity, TKey}.GetObjects(TKey[])" />.
	/// </summary>
	Func<DbContext, TKey[], IEnumerable<TEntity>> GetGetObjectsQuery();

	/// <summary>
	/// Vrátí dotaz pro metodu <see cref="DbRepository{TEntity, TKey}.GetObjectsAsync(TKey[], CancellationToken)" />.
	/// </summary>
	Func<DbContext, TKey[], IAsyncEnumerable<TEntity>> GetGetObjectsAsyncQuery();
}