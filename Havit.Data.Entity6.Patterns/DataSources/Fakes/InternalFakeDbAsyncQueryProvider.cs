using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataSources.Fakes;

/// <summary>
/// Vzor: https://msdn.microsoft.com/cs-cz/data/dn314429#async
/// </summary>
internal class InternalFakeDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
{
	private readonly IQueryProvider _inner;

	internal InternalFakeDbAsyncQueryProvider(IQueryProvider inner)
	{
		_inner = inner;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		return new InternalFakeDbAsyncEnumerable<TEntity>(expression);
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return new InternalFakeDbAsyncEnumerable<TElement>(expression);
	}

	public object Execute(Expression expression)
	{
		return _inner.Execute(expression);
	}

	public TResult Execute<TResult>(Expression expression)
	{
		return _inner.Execute<TResult>(expression);
	}

	public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
	{
		return Task.FromResult(Execute(expression));
	}

	public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
	{
		return Task.FromResult(Execute<TResult>(expression));
	}
}