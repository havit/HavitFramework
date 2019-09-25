using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes
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

		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			return new AsyncEnumeratorInternal<T>(this.AsEnumerable().GetEnumerator());
		}
	}
}