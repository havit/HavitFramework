using Havit.Diagnostics.Contracts;

namespace Havit.Linq;

/// <summary>
/// Extension methods for IEnumerable&lt;T&gt;.
/// </summary>
public static class EnumerableExt
{
	/// <summary>
	/// Optionally extends the query according to source with a condition based on predicate if condition is true.
	/// If condition is false, returns the unmodified query.
	/// </summary>
	/// <typeparam name="TSource">Type of object in source.</typeparam>
	/// <param name="source">Extended query.</param>
	/// <param name="condition">Condition determining whether to extend the query with a condition.</param>
	/// <param name="predicate">Condition that optionally extends the query.</param>
	/// <returns>Query optionally extended with a condition.</returns>
	public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
	{
		return condition
			? source.Where(predicate)
			: source;
	}

	/// <summary>
	/// Left outer join.
	/// </summary>
	public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftSource,
											 IEnumerable<TRight> rightSource,
											 Func<TLeft, TKey> leftKeySelector,
											 Func<TRight, TKey> rightKeySelector,
											 Func<TLeft, TRight, TResult> resultSelector)
	{
		IEnumerable<TResult> result =
			from l in leftSource
			join r in rightSource
			on leftKeySelector(l) equals rightKeySelector(r) into joinData
			from right in joinData.DefaultIfEmpty()
			select resultSelector(l, right);

		return result;
	}

	/// <summary>
	/// Right outer join.
	/// </summary>
	public static IEnumerable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftSource,
											 IEnumerable<TRight> rightSource,
											 Func<TLeft, TKey> leftKeySelector,
											 Func<TRight, TKey> rightKeySelector,
											 Func<TLeft, TRight, TResult> resultSelector)
	{
		IEnumerable<TResult> result =
			from r in rightSource
			join l in leftSource
			on rightKeySelector(r) equals leftKeySelector(l) into joinData
			from left in joinData.DefaultIfEmpty()
			select resultSelector(left, r);

		return result;
	}

	/// <summary>
	/// Full outer join.
	/// Unlike other methods, full outer join is evaluated immediately, results are not affected by changes 
	/// in source data (leftSource, rightSource) after calling this method and before actual query evaluation (IEnumerable).
	/// </summary>
	public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
		this IEnumerable<TLeft> leftSource,
		IEnumerable<TRight> rightSource,
		Func<TLeft, TKey> leftKeySelector,
		Func<TRight, TKey> rightKeySelector,
		Func<TLeft, TRight, TResult> resultSelector)
	{
		return FullOuterJoin(leftSource, rightSource, leftKeySelector, rightKeySelector, resultSelector, EqualityComparer<TKey>.Default);
	}

	/// <summary>
	/// Full outer join.
	/// Unlike other methods, full outer join is evaluated immediately, results are not affected by changes 
	/// in source data (leftSource, rightSource) after calling this method and before actual query evaluation (IEnumerable).
	/// </summary>
	public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
		this IEnumerable<TLeft> leftSource,
		IEnumerable<TRight> rightSource,
		Func<TLeft, TKey> leftKeySelector,
		Func<TRight, TKey> rightKeySelector,
		Func<TLeft, TRight, TResult> resultSelector,
		IEqualityComparer<TKey> equalityComparer)
	{
		var leftLookup = leftSource.ToLookup(leftKeySelector, equalityComparer);
		var rightLookup = rightSource.ToLookup(rightKeySelector, equalityComparer);

		var keys = new HashSet<TKey>(leftLookup.Select(p => p.Key), equalityComparer);
		keys.UnionWith(rightLookup.Select(p => p.Key));

		IEnumerable<TResult> result = from key in keys
									  from xLeft in leftLookup[key].DefaultIfEmpty()
									  from xRight in rightLookup[key].DefaultIfEmpty()
									  select resultSelector(xLeft, xRight);

		return result;
	}

	/// <summary>
	/// Skip last items.
	/// </summary>
#if NET6_0_OR_GREATER
	// System.Linq in .NET 6 contains this extension method. When used, the error "The call is ambiguous between the following methods..." is reported.
	// This could be resolved by removing this method from .NET 6, however, it can introduce an unpleasant aspect, such as:
	// Consider a library implemented for .NET Standard 2.0, which uses this extension method SkipLast.
	// However, .NET 6 is chosen as the runtime of the project using this library.
	// Everything will compile without errors, but in runtime, we get a MissingMethodException, indicating the method is not in the assembly.

	// As a solution, we choose to have this method in .NET 6, but not as an extension method.
	// This allows us to use the syntactic sugar of the extension method in .NET Standard 2.0 and our method is used.
	// When running under .NET 6, this method is used, it is found, and the fact that it is not an extension method does not matter.
	// Compilation of SkipLast(...) calls in .NET 6 will use the extension method from System.Linq.Enumerable.
	public static IEnumerable<TSource> SkipLast<TSource>(IEnumerable<TSource> source, int count)
#else
	public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
#endif
	{
		if (count <= 0)
		{
			return source.Select(item => item);
		}

		return SkipLastInternal(source, count);
	}

	private static IEnumerable<TSource> SkipLastInternal<TSource>(IEnumerable<TSource> source, int count)
	{
		if (source is IList<TSource>)
		{
			IList<TSource> sourceList = (IList<TSource>)source;
			int sourceItems = sourceList.Count;

			for (int i = 0; i < (sourceItems - count); i++)
			{
				yield return sourceList[i];
			}
		}
		else
		{
			var sourceEnumerator = source.GetEnumerator();
			var buffer = new TSource[count];
			int idx;

			for (idx = 0; (idx < count) && sourceEnumerator.MoveNext(); idx++)
			{
				buffer[idx] = sourceEnumerator.Current;
			}

			idx = 0;
			while (sourceEnumerator.MoveNext())
			{
				var item = buffer[idx];

				buffer[idx] = sourceEnumerator.Current;

				idx = (idx + 1) % count;

				yield return item;
			}
		}
	}

	/// <summary>
	/// Skip last items.
	/// </summary>
	public static IEnumerable<TSource> SkipLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		var buffer = new List<TSource>();

		foreach (var item in source)
		{
			if (predicate(item))
			{
				buffer.Add(item);
			}
			else
			{
				if (buffer.Count > 0)
				{
					foreach (var bufferedItem in buffer)
					{
						yield return bufferedItem;
					}

					buffer.Clear();
				}

				yield return item;
			}
		}
	}

	/// <summary>
	/// Skip last items.
	/// </summary>
	public static IEnumerable<TSource> SkipLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		var buffer = new List<TSource>();
		var idx = 0;

		foreach (var item in source)
		{
			if (predicate(item, idx++))
			{
				buffer.Add(item);
			}
			else
			{
				if (buffer.Count > 0)
				{
					foreach (var bufferedItem in buffer)
					{
						yield return bufferedItem;
					}

					buffer.Clear();
				}

				yield return item;
			}
		}
	}

	/// <summary>
	/// Splits data into segments (chunks) of maximum size according to size.
	/// For example, input data of 2500 records at size 1000 will be split into three segments (chunks) - 1000, 1000, and 500 records.
	/// </summary>
	/// <param name="source">Source data.</param>
	/// <param name="size">Size of one segment (chunk). The smallest possible value is 1.</param>
#if NET6_0_OR_GREATER
	[Obsolete("Use the Chunk method instead of Chunkify. The Chunk method was introduced in LINQ with .NET 6.")]
#endif
	public static IEnumerable<T[]> Chunkify<T>(this IEnumerable<T> source, int size)
	{
		Contract.Requires<ArgumentNullException>(source != null, nameof(source));
		Contract.Requires<ArgumentOutOfRangeException>(size > 0, nameof(size));

		using (var iter = source.GetEnumerator())
		{
			while (iter.MoveNext())
			{
				var chunk = new T[size];
				chunk[0] = iter.Current;
				int i;
				for (i = 1; i < size && iter.MoveNext(); i++)
				{
					chunk[i] = iter.Current;
				}

				if (size != i)
				{
					Array.Resize(ref chunk, i);
				}
				yield return chunk;
			}
		}
	}

	/// <summary>
	/// Indicates whether the collection contains all items of another collection.
	/// </summary>
	/// <param name="source">The collection in which we verify the existence of values.</param>
	/// <param name="lookupItems">The values whose existence we verify in the collection.</param>
	/// <returns>True if the source contains all elements of lookupItems.</returns>
	public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> lookupItems)
	{
		return !lookupItems.Except(source).Any();
	}
}
