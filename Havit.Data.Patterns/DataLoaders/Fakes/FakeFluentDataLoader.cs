using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders.Fakes;

/// <summary>
/// FluentAPI pro explicity data loader, který nic nedělá.
/// </summary>
public class FakeFluentDataLoader<TEntity> : Havit.Data.Patterns.DataLoaders.IFluentDataLoader<TEntity>
	where TEntity : class
{
	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	IFluentDataLoader<TProperty> IFluentDataLoader<TEntity>.Load<TProperty>(Expression propertyPath)
	{
		return new FakeFluentDataLoader<TProperty>();
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	Task<IFluentDataLoader<TProperty>> IFluentDataLoader<TEntity>.LoadAsync<TProperty>(Expression propertyPath, CancellationToken cancellationToken /* no default */)
	{
		return Task.FromResult((IFluentDataLoader<TProperty>)new FakeFluentDataLoader<TProperty>());
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	IFluentDataLoader<TWrappedEntity> IFluentDataLoader<TEntity>.Unwrap<TWrappedEntity>()
	{
		return new FakeFluentDataLoader<TWrappedEntity>();
	}
}