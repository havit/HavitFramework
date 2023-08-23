using System.Collections.Concurrent;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Úložiště dotazů pro použití v repository.
/// </summary>
public interface IRepositoryQueryStore
{
	/// <summary>
	/// Úložiště dotazů pro metodu <see cref="DbRepository{TEntity}.GetObject(int)" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetObjectCompiledQueries { get; }

	/// <summary>
	/// Úložiště dotazů pro metodu <see cref="DbRepository{TEntity}.GetObjectAsync(int, CancellationToken)" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetObjectAsyncCompiledQueries { get; }

	/// <summary>
	/// Úložiště dotazů pro metodu <see cref="DbRepository{TEntity}.GetObjects(int[])" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetObjectsCompiledQueries { get; }

	/// <summary>
	/// Úložiště dotazů pro metodu <see cref="DbRepository{TEntity}.GetObjectsAsync(int[], CancellationToken)" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetObjectsAsyncCompiledQueries { get; }

	/// <summary>
	/// Úložiště dotazů pro metodu <see cref="DbRepository{TEntity}.GetAll" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetAllCompiledQueries { get; }

	/// <summary>
	/// Úložiště
	/// dotazů pro metodu <see cref="DbRepository{TEntity}.GetAllAsync(CancellationToken)" />.
	/// </summary>
	ConcurrentDictionary<Type, object> GetAllAsyncCompiledQueries { get; }
}