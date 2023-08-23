using System.Collections.Concurrent;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <inheritdoc />
public class RepositoryQueryStore : IRepositoryQueryStore
{
	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetObjectCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();

	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetObjectAsyncCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();

	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetObjectsCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();

	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetObjectsAsyncCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();

	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetAllCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();

	/// <inheritdoc />
	public ConcurrentDictionary<Type, object> GetAllAsyncCompiledQueries { get; } = new ConcurrentDictionary<Type, object>();
}
