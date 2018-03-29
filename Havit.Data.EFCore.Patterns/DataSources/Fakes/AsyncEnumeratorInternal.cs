using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataSources.Fakes
{
	internal class AsyncEnumeratorInternal<T> : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> source;

		public AsyncEnumeratorInternal(IEnumerator<T> source)
		{
			this.source = source;
		}

		public T Current => source.Current;

		public Task<bool> MoveNext(CancellationToken cancellationToken)
		{
			return Task.FromResult(source.MoveNext());
		}

		public void Dispose()
		{
			source.Dispose();
		}
	}
}