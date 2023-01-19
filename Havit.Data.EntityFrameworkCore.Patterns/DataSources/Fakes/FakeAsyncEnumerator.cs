using System.Collections.Generic;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes
{
	internal class FakeAsyncEnumerator<T> : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public FakeAsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
		}

		public T Current
		{
			get
			{
				return _inner.Current;
			}
		}

		public ValueTask<bool> MoveNextAsync()
		{
			return new ValueTask<bool>(_inner.MoveNext());
		}

		public ValueTask DisposeAsync()
		{
			_inner.Dispose();
			return default;
		}
	}
}
