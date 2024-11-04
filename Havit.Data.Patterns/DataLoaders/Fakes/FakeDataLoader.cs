using Havit.Data.Patterns.Attributes;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders.Fakes;

/// <summary>
/// Explicity data loader, který nic nedělá.
/// Určeno pro použití v unit testech pro mock IDataLoaderu.
/// </summary>
[Fake]
public class FakeDataLoader : IDataLoader
{
	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public IFluentDataLoader<TProperty> Load<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
		where TProperty : class
	{
		return new NullFluentDataLoader<TProperty>();
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public void Load<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
		where TEntity : class
	{
		// NOOP
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public IFluentDataLoader<TProperty> LoadAll<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
		where TProperty : class
	{
		return new NullFluentDataLoader<TProperty>();
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public void LoadAll<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
		where TEntity : class
	{
		// NOOP
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public ValueTask<IFluentDataLoader<TProperty>> LoadAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken = default)
		where TEntity : class
		where TProperty : class
	{
		return new ValueTask<IFluentDataLoader<TProperty>>(new NullFluentDataLoader<TProperty>());
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public ValueTask LoadAsync<TEntity>(TEntity entity, Expression<Func<TEntity, object>>[] propertyPaths, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		// TODO: .NET5+ ValueTask.CompletedTask
		return new ValueTask();
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public ValueTask<IFluentDataLoader<TProperty>> LoadAllAsync<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken = default)
		where TEntity : class
		where TProperty : class
	{
		return new ValueTask<IFluentDataLoader<TProperty>>(new NullFluentDataLoader<TProperty>());
	}

	/// <summary>
	/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
	/// Implementace: Nic nedělá.
	/// </summary>
	public ValueTask LoadAllAsync<TEntity>(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[] propertyPaths, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		// TODO: .NET5+ ValueTask.CompletedTask
		return new ValueTask();
	}
}
