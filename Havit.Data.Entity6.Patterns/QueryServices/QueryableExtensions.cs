using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Entity.Patterns.QueryServices
{
	public static class QueryableExtensions
	{
		// zveřejňuje Include i uživatele této knihovny, kteří nemusí mít EF (Include je definován v EF)
		public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> path)
		{
			return System.Data.Entity.QueryableExtensions.Include(source, path);
		}
	}
}
