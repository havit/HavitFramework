using Microsoft.EntityFrameworkCore;
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
	private readonly DbContext _dbContext;
	private readonly DbSet<TEntity> _dbSet;

	private IKey _primaryKey;
#pragma warning disable EF1001 // Internal EF Core API usage.
	private IStateManager _stateManager;
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbSetInternal(DbContext dbContext)
	{
		_dbContext = dbContext;
		_dbSet = dbContext.Set<TEntity>();
	}

	/// <inheritdoc />
	public IQueryable<TEntity> AsQueryable(string queryTag)
	{
		return !String.IsNullOrEmpty(queryTag)
			? _dbSet.TagWith(queryTag)
			: _dbSet;
	}

	/// <summary>
	/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
	/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
	public TEntity FindTracked(params object[] keyValues)
	{
		_primaryKey ??= _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();
		_stateManager ??= _dbContext.GetService<IStateManager>();

		return (TEntity)_stateManager.TryGetEntry(_primaryKey, keyValues)?.Entity;
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <inheritdoc />
	public void Add(TEntity entity)
	{
		_dbSet.Add(entity);
	}

	/// <inheritdoc />
	public void AddRange(IEnumerable<TEntity> entities)
	{
		_dbSet.AddRange(entities);
	}

	/// <inheritdoc />
	public void Update(TEntity entity)
	{
		_dbSet.Update(entity);
	}

	/// <inheritdoc />
	public void UpdateRange(IEnumerable<TEntity> entities)
	{
		_dbSet.UpdateRange(entities);
	}

	/// <inheritdoc />
	public void Remove(TEntity entity)
	{
		_dbSet.Remove(entity);
	}

	/// <inheritdoc />
	public void RemoveRange(IEnumerable<TEntity> entities)
	{
		_dbSet.RemoveRange(entities);
	}

	/// <inheritdoc />
	public void Attach(TEntity entity)
	{
		_dbSet.Attach(entity);
	}

	/// <inheritdoc />
	public void AttachRange(IEnumerable<TEntity> entities)
	{
		_dbSet.AttachRange(entities);
	}

}