﻿namespace Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;

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
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(softDeleteManager);

		return softDeleteManager.IsSoftDeleteSupported<TSource>()
			? source.Where(softDeleteManager.GetNotDeletedExpressionLambda<TSource>())
			: source;
	}

	/// <summary>
	/// Vrací z dat pouze záznamy, které nejsou smazané příznakem.
	/// </summary>
	public static IEnumerable<TSource> WhereNotDeleted<TSource>(this IEnumerable<TSource> source, ISoftDeleteManager softDeleteManager)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(softDeleteManager);

		return softDeleteManager.IsSoftDeleteSupported<TSource>()
			? source.Where(softDeleteManager.GetNotDeletedCompiledLambda<TSource>())
			: source;
	}
}
