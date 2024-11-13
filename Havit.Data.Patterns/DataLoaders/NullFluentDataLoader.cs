using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders;

/// <summary>
/// Prázdná implementace IFluentDataLoaderu, nic nenačítá.
/// </summary>
public class NullFluentDataLoader<TEntity> : IFluentDataLoader<TEntity>
	where TEntity : class
{
	/// <summary>
	/// Nedělá nic.
	/// </summary>
	public IFluentDataLoader<TProperty> Load<TProperty>(Expression propertyPath) where TProperty : class
	{
		return new NullFluentDataLoader<TProperty>();
	}

	/// <summary>
	/// Nedělá nic.
	/// </summary>
	public Task<IFluentDataLoader<TProperty>> LoadAsync<TProperty>(Expression propertyPath, CancellationToken cancellationToken = default) where TProperty : class
	{
		return Task.FromResult((IFluentDataLoader<TProperty>)new NullFluentDataLoader<TProperty>());
	}

	/// <summary>
	/// Nedělá nic.
	/// </summary>
	public IFluentDataLoader<TWrappedEntity> Unwrap<TWrappedEntity>() where TWrappedEntity : class
	{
		return new NullFluentDataLoader<TWrappedEntity>();
	}
}
