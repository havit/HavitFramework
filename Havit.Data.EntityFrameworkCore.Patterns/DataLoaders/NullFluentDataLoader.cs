using System.Linq.Expressions;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataLoaders.Fakes;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

internal class NullFluentDataLoader<TEntity> : IFluentDataLoader<TEntity>
	where TEntity : class
{
	public IFluentDataLoader<TProperty> Load<TProperty>(Expression propertyPath) where TProperty : class
	{
		return new NullFluentDataLoader<TProperty>();
	}

	public Task<IFluentDataLoader<TProperty>> LoadAsync<TProperty>(Expression propertyPath, CancellationToken cancellationToken = default) where TProperty : class
	{
		return Task.FromResult((IFluentDataLoader<TProperty>)new NullFluentDataLoader<TProperty>());
	}

	public IFluentDataLoader<TWrappedEntity> Unwrap<TWrappedEntity>() where TWrappedEntity : class
	{
		return new NullFluentDataLoader<TWrappedEntity>();
	}
}
