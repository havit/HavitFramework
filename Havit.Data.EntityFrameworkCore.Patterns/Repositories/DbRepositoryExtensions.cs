using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Extension methods related to DbRepositories.
/// </summary>
public static class DbRepositoryExtensions
{
	/// <summary>
	/// To be used inside DbRepository implementations as .Include(GetLoadReferences).MyQueryContinued...
	/// </summary>
	public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> data, Func<IEnumerable<Expression<Func<TEntity, object>>>> getLoadReferencesFunc)
		where TEntity : class
	{
		foreach (var includePath in getLoadReferencesFunc.Invoke())
		{
			data = data.Include(includePath);
		}

		return data;
	}
}