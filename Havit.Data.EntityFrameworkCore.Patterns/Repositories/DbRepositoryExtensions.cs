using Havit.Data.Patterns.Exceptions;
using Havit.Data.Patterns.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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