using System;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Operace pro konfiguraci seedování dat.
/// </summary>
public interface IDataSeedForPaired<TEntity> : IDataSeedFor<TEntity>
	where TEntity : class
{
	/// <summary>
	/// Nastaví způsob párování dat.
	/// </summary>
	IDataSeedForPaired<TEntity> AndBy(params Expression<Func<TEntity, object>>[] andByExpressions);
}