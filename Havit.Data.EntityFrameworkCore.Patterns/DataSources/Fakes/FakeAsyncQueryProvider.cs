using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

internal class FakeAsyncQueryProvider<TEntity> : Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider
{
	private readonly IQueryProvider _inner;

	internal FakeAsyncQueryProvider(IQueryProvider inner)
	{
		_inner = inner;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		return new FakeAsyncEnumerable<TEntity>(expression);
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return new FakeAsyncEnumerable<TElement>(expression);
	}

	public object Execute(Expression expression)
	{
		return _inner.Execute(expression);
	}

	public TResult Execute<TResult>(Expression expression)
	{
		return _inner.Execute<TResult>(expression);
	}

	TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
	{
		return _inner.Execute<TResult>(expression);
	}
}
