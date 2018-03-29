using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataSources.Fakes
{
	/// <summary>
	/// Podpora asychronním operací pro "in-memory queryable" - simulace IQueryable&lt;&gt; v paměti.
	/// </summary>
	internal class AsyncEnumerableQueryInternal<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
	{ 
		public AsyncEnumerableQueryInternal(IEnumerable<T> enumerable) 
			: base(enumerable) 
		{ }

		public AsyncEnumerableQueryInternal(Expression expression)
			: base(expression)
		{ } 
 
		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
		{
			return this.AsAsyncEnumerable().GetEnumerator();
		}
	}
}