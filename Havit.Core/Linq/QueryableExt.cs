using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Linq
{
	/// <summary>
	/// Extension metody pro IQueryable&lt;T&gt;.
	/// </summary>
	public static class QueryableExt
	{
		/// <summary>
		/// Volitelně rozšíří dotaz dle source o podmínku dle predicate, pokud condition je true.
		/// Pokud je condition false, vrátí nezměněný dotaz.
		/// </summary>
		/// <typeparam name="TSource">Typ objektu v source.</typeparam>
		/// <param name="source">Rozšiřovaný dotaz.</param>
		/// <param name="condition">Podmínka určující, zda má být dotaz rozšířen o podmínku.</param>
		/// <param name="predicate">Podmínka, která volitelně rozšiřuje dotaz.</param>
		/// <returns>Dotaz volitelně rozšířený o podmínku.</returns>
		public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
		{
			return condition
				? source.Where(predicate)
				: source;
		}
	}
}
