using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes
{
	internal class FakeAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public FakeAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        {
            // NOOP
        }

        public FakeAsyncEnumerable(Expression expression) : base(expression)
        {
            // NOOP
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
            return new FakeAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

		IQueryProvider IQueryable.Provider
		{
			get { return new FakeAsyncQueryProvider<T>(this); }
		}
	}
}
