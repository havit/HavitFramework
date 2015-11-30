using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.SoftDeletes
{
	/// <summary>
	/// Extension metody pro snadné filtrování IQueryable&lt;T&gt; a IEnumerable&lt;T&gt; s pomocí ISoftDeleteManager.
	/// </summary>
	public static class SoftDeleteManagerExtensions
	{
		/// <summary>
		/// Rozšíří dotaz o odstranění smazaných objektů.
		/// </summary>
		public static IQueryable<TSource> WhereNotDeleted<TSource>(this IQueryable<TSource> source, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires(source != null);
			Contract.Requires(softDeleteManager != null);

			if (softDeleteManager.IsSoftDeleteSupported<TSource>())
			{
				return source.Where(softDeleteManager.GetNotDeletedExpression<TSource>());
			}
			else
			{
				return source;
			}
		}

		/// <summary>
		/// Vrací z dat pouze záznamy, které nejsou smazané příznakem.
		/// </summary>
		public static IEnumerable<TSource> WhereNotDeleted<TSource>(this IEnumerable<TSource> source, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires(source != null);
			Contract.Requires(softDeleteManager != null);

			if (softDeleteManager.IsSoftDeleteSupported<TSource>())
			{
				return source.Where(softDeleteManager.GetNotDeletedExpression<TSource>().Compile());
			}
			else
			{
				return source;
			}
		}
	}
}
