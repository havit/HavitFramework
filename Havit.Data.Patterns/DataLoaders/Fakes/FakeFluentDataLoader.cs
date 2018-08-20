using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders.Fakes
{
	/// <summary>
	/// FluentAPI pro explicity data loader, který nic nedělá.
	/// </summary>
	public class FakeFluentDataLoader<TEntity> : Havit.Data.Patterns.DataLoaders.IFluentDataLoader<TEntity>, IFluentDataLoaderAsync<TEntity>
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
		Task<IFluentDataLoaderAsync<TProperty>> IFluentDataLoaderAsync<TEntity>.LoadAsync<TProperty>(Expression propertyPath)
		{
			return Task.FromResult((IFluentDataLoaderAsync<TProperty>)new FakeFluentDataLoader<TProperty>());			
		}

		/// <summary>
		/// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
		/// Implementace: Nic nedělá.
		/// </summary>
		IFluentDataLoaderAsync<TWrappedEntity> IFluentDataLoaderAsync<TEntity>.Unwrap<TWrappedEntity>()
		{
			return new FakeFluentDataLoader<TWrappedEntity>();
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
}