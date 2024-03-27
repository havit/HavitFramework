using Microsoft.EntityFrameworkCore;
#if BENCHMARKING
using Microsoft.EntityFrameworkCore.ChangeTracking;
#endif
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Internal;

/// <summary>
/// Poskytuje služby DbSetu pod interfacem IDbSet&lt;TEntity&gt;.
/// </summary>
internal class DbSetInternal<TEntity> : IDbSet<TEntity>
	where TEntity : class
{
	private readonly DbSet<TEntity> dbSet;
	private readonly Lazy<IKey> primaryKeyLazy;
	private readonly Lazy<IStateManager> stateManagerLazy;
#if BENCHMARKING
	private readonly Lazy<LocalView<TEntity>> localViewLazy;
#endif

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbSetInternal(DbContext dbContext)
	{
		this.dbSet = dbContext.Set<TEntity>();
		this.primaryKeyLazy = new Lazy<IKey>(() => dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey(), LazyThreadSafetyMode.None);
#pragma warning disable EF1001 // Internal EF Core API usage.
		this.stateManagerLazy = new Lazy<IStateManager>(() => dbContext.GetService<IStateManager>(), LazyThreadSafetyMode.None);
#pragma warning restore EF1001 // Internal EF Core API usage.
#if BENCHMARKING
		this.localViewLazy = new Lazy<LocalView<TEntity>>(() => dbContext.ExecuteWithoutAutoDetectChanges(() => dbSet.Local));
#endif
	}

	/// <inheritdoc />
	public IQueryable<TEntity> AsQueryable(string queryTag)
	{
		return !String.IsNullOrEmpty(queryTag)
			? dbSet.TagWith(queryTag)
			: dbSet;
	}

	/// <inheritdoc />
	public DbSet<TEntity> AsDbSet() => dbSet;

	/// <summary>
	/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
	/// </summary>
	public TEntity FindTracked(params object[] keyValues)
	{
#pragma warning disable EF1001 // Internal EF Core API usage.
		return (TEntity)stateManagerLazy.Value.TryGetEntry(primaryKeyLazy.Value, keyValues)?.Entity;
#pragma warning restore EF1001 // Internal EF Core API usage.
	}

#if BENCHMARKING
	internal TEntity UsingLocal_FindEntry<TKey>(TKey key)
	{
		return localViewLazy.Value.FindEntry(key)?.Entity;
	}

	internal TEntity UsingLocal_FindEntryUntyped(params object[] keyValues)
	{
		return localViewLazy.Value.FindEntryUntyped(keyValues)?.Entity;
	}
#endif

	/// <inheritdoc />
	public void Add(TEntity entity)
	{
		dbSet.Add(entity);
	}

	/// <inheritdoc />
	public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		await dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public void AddRange(TEntity[] entities)
	{
		dbSet.AddRange(entities);
	}

	/// <inheritdoc />
	public async Task AddRangeAsync(TEntity[] entities, CancellationToken cancellationToken = default)
	{
		await dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public void Update(TEntity entity)
	{
		dbSet.Update(entity);
	}

	/// <inheritdoc />
	public void UpdateRange(TEntity[] entities)
	{
		dbSet.UpdateRange(entities);
	}

	/// <inheritdoc />
	public void Remove(TEntity entity)
	{
		dbSet.Remove(entity);
	}

	/// <inheritdoc />
	public void RemoveRange(TEntity[] entities)
	{
		dbSet.RemoveRange(entities);
	}

	/// <inheritdoc />
	public void Attach(TEntity entity)
	{
		dbSet.Attach(entity);
	}

	/// <inheritdoc />
	public void AttachRange(TEntity[] entities)
	{
		dbSet.AttachRange(entities);
	}

}