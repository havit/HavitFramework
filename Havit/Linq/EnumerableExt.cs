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
		#region LeftJoin
		/// <summary>
		/// Left outer join.
		/// </summary>
		public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftSource,
												 IEnumerable<TRight> rightSource,
												 Func<TLeft, TKey> leftKeySelector,
												 Func<TRight, TKey> rightKeySelector,
												 Func<TLeft, TRight, TResult> resultSelector)
		{
			IEnumerable<TResult> _result =
				from l in leftSource
				join r in rightSource
				on leftKeySelector(l) equals rightKeySelector(r) into joinData
				from right in joinData.DefaultIfEmpty()
				select resultSelector(l, right);

			return _result;
		}
		#endregion

		#region RightJoin
		/// <summary>
		/// Right outer join.
		/// </summary>
		public static IEnumerable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftSource,
												 IEnumerable<TRight> rightSource,
												 Func<TLeft, TKey> leftKeySelector,
												 Func<TRight, TKey> rightKeySelector,
												 Func<TLeft, TRight, TResult> resultSelector)
		{
			IEnumerable<TResult> _result =
				from r in rightSource
				join l in leftSource
				on rightKeySelector(r) equals leftKeySelector(l) into joinData
				from left in joinData.DefaultIfEmpty()
				select resultSelector(left, r);

			return _result;
		}
		#endregion

		#region FullOuterJoin
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

			IEnumerable<TResult> _result = from key in keys
					   from xLeft in leftLookup[key].DefaultIfEmpty()
					   from xRight in rightLookup[key].DefaultIfEmpty()
					   select resultSelector(xLeft, xRight);

			return _result.ToList();
		}
		#endregion
	}
}
