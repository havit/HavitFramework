using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

internal class DbRepositoryCompiledQueryStore
{
	/// <summary>
	/// Jediná instance, používaná z DbRepository.
	/// </summary>
	public static DbRepositoryCompiledQueryStore Default { get; } = new DbRepositoryCompiledQueryStore();

	private DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

	private ConcurrentDictionary<Type, object> getObjectCompiledQueries = new ConcurrentDictionary<Type, object>();
	private ConcurrentDictionary<Type, object> getObjectAsyncCompiledQueries = new ConcurrentDictionary<Type, object>();
	private ConcurrentDictionary<Type, object> getObjectsCompiledQueries = new ConcurrentDictionary<Type, object>();
	private ConcurrentDictionary<Type, object> getObjectsAsyncCompiledQueries = new ConcurrentDictionary<Type, object>();
	private ConcurrentDictionary<Type, object> getAllCompiledQueries = new ConcurrentDictionary<Type, object>();
	private ConcurrentDictionary<Type, object> getAllAsyncCompiledQueries = new ConcurrentDictionary<Type, object>();

	public Func<DbContext, int, TEntity> GetGetObjectCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int, TEntity>)getObjectCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetObjectCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	public Func<DbContext, int, CancellationToken, Task<TEntity>> GetGetObjectAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int, CancellationToken, Task<TEntity>>)getObjectAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetObjectAsyncCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	public Func<DbContext, int[], IEnumerable<TEntity>> GetGetObjectsCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
	where TEntity : class
	{
		return (Func<DbContext, int[], IEnumerable<TEntity>>)getObjectsCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetObjectsCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	public Func<DbContext, int[], IAsyncEnumerable<TEntity>> GetGetObjectsAsyncCompiledQuery<TEntity>(Type repositoryType, IEntityKeyAccessor<TEntity, int> entityKeyAccessor)
		where TEntity : class
	{
		return (Func<DbContext, int[], IAsyncEnumerable<TEntity>>)getObjectsAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetObjectsAsyncCompiledQuery<TEntity>(repositoryType, entityKeyAccessor));
	}

	public Func<DbContext, IEnumerable<TEntity>> GetGetAllCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return (Func<DbContext, IEnumerable<TEntity>>)getAllCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetAllCompiledQuery<TEntity>(repositoryType, softDeleteManager));
	}

	public Func<DbContext, IAsyncEnumerable<TEntity>> GetGetAllAsyncCompiledQuery<TEntity>(Type repositoryType, ISoftDeleteManager softDeleteManager)
		where TEntity : class
	{
		return (Func<DbContext, IAsyncEnumerable<TEntity>>)getAllAsyncCompiledQueries.GetOrAdd(typeof(TEntity), _ => compiledQueryBuilder.CreateGetAllAsyncCompiledQuery<TEntity>(repositoryType, softDeleteManager));
	}

}
