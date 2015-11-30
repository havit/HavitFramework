using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.QueryServices.Fakes
{
	/// <summary>
	/// Vzor: https://msdn.microsoft.com/cs-cz/data/dn314429#async
	/// </summary>
	internal class InternalFakeDbAsyncEnumerator<T> : IDbAsyncEnumerator<T> 
	{ 
		private readonly IEnumerator<T> innerEnumerator;

		public T Current
		{
			get { return innerEnumerator.Current; }
		} 

		public InternalFakeDbAsyncEnumerator(IEnumerator<T> innerEnumerator) 
		{ 
			this.innerEnumerator = innerEnumerator; 
		}

		object IDbAsyncEnumerator.Current
		{
			get { return Current; }
		}

		Task<bool> IDbAsyncEnumerator.MoveNextAsync(CancellationToken cancellationToken) 
		{ 
			return Task.FromResult(innerEnumerator.MoveNext()); 
		}

		void IDisposable.Dispose()
		{
			innerEnumerator.Dispose();
		}

	}
}