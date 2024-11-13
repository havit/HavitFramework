﻿using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataSources;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources;

/// <summary>
/// Poskytuje datový zdroj objektů TEntity jako IQueryable.
/// </summary>
public abstract class DbDataSource<TEntity> : IDataSource<TEntity>
	where TEntity : class
{
	private readonly ISoftDeleteManager _softDeleteManager;
	private readonly Lazy<IDbSet<TEntity>> _dbSetLazy;

	/// <summary>
	/// Vrací data z datového zdroje jako IQueryable.
	/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
	/// </summary>
	public virtual IQueryable<TEntity> Data => _dbSetLazy.Value.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(Data))).WhereNotDeleted(_softDeleteManager);

	/// <summary>
	/// Vrací data z datového zdroje jako IQueryable.
	/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
	/// </summary>
	public virtual IQueryable<TEntity> DataIncludingDeleted => _dbSetLazy.Value.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(DataIncludingDeleted)));

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected DbDataSource(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
	{
		this._dbSetLazy = new Lazy<IDbSet<TEntity>>(() => dbContext.Set<TEntity>(), LazyThreadSafetyMode.None);
		this._softDeleteManager = softDeleteManager;
	}
}
