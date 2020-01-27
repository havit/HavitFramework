using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Linq
{
	/// <summary>
	/// Extension metody pro IEnumerable&lt;T&gt;.
	/// </summary>
	public static class EnumerableExt
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
		/// Na rozdíl od ostatních metod je full outer join vyhodnocen okamžitě, výsledky nejsou ovlivněny změnou 
		/// source dat (leftSource, rightSource) za zavoláním této metody a před vlastním vyhodnocením dotazu (IEnumerable).
		/// </summary>
		public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftSource,
												 IEnumerable<TRight> rightSource,
												 Func<TLeft, TKey> leftKeySelector,
												 Func<TRight, TKey> rightKeySelector,
												 Func<TLeft, TRight, TResult> resultSelector)
		{
			var leftLookup = leftSource.ToLookup(leftKeySelector);
			var rightLookup = rightSource.ToLookup(rightKeySelector);

			var keys = new HashSet<TKey>(leftLookup.Select(p => p.Key));
			keys.UnionWith(rightLookup.Select(p => p.Key));

			IEnumerable<TResult> result = from key in keys
					   from xLeft in leftLookup[key].DefaultIfEmpty()
					   from xRight in rightLookup[key].DefaultIfEmpty()
					   select resultSelector(xLeft, xRight);

			return result.ToList();
		}

		/// <summary>
		/// Skip last items.
		/// </summary>
		public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
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
		/// Rozdělí data do segmentů (chunks) o maximální velikosti dle size.
		/// Například vstupní data o 2500 záznamech při size 1000 rozdělí do třech segmentů (chunků) - 1000, 1000 a 500 záznamů.
		/// </summary>
		/// <param name="source">Zdrojová data.</param>
		/// <param name="size">Velikost jednoho segmentu (chunku). Nejmenší možná hodnota he 1.</param>
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
	    /// Indikuje, zda obsahuje kolekce všechny položky jiné kolekce.
	    /// </summary>
	    /// <param name="source">Kolekce, v níž ověřujeme existenci hodnot.</param>
	    /// <param name="lookupItems">Hodnoty, jejichž existenci ověřujeme v kolekci.</param>
	    /// <returns>True, pokud source obsahuje všechny prvky lookupItems.</returns>
	    public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> lookupItems)
	    {
	        return !lookupItems.Except(source).Any();
	    }
    }
}
