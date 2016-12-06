using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataSources.Fakes
{
	/// <summary>
	/// Vzor: https://msdn.microsoft.com/cs-cz/data/dn314429#async
	/// </summary>
	internal class InternalFakeDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T> 
	{ 
		public InternalFakeDbAsyncEnumerable(IEnumerable<T> enumerable) 
			: base(enumerable) 
		{ }

		public InternalFakeDbAsyncEnumerable(Expression expression)
			: base(expression)
		{ } 
 
		public IDbAsyncEnumerator<T> GetAsyncEnumerator() 
		{ 
			return new InternalFakeDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator()); 
		} 
 
		IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() 
		{ 
			return GetAsyncEnumerator(); 
		} 
 
		IQueryProvider IQueryable.Provider 
		{ 
			get { return new InternalFakeDbAsyncQueryProvider<T>(this); } 
		} 
	}
}