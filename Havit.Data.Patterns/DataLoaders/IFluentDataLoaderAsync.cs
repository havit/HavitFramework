using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Fluent API pro data loader.
	/// Umožňuje zřetězený zápis pro načítání vlastností data loaderem.
	/// </summary>
	public interface IFluentDataLoaderAsync<out TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// Není určeno pro volání z klientského kódu.
		/// </summary>
		/// <param name="propertyPath">
		/// Vlastnost, která má být načtena.
		/// </param>
		Task<IFluentDataLoaderAsync<TProperty>> LoadAsync<TProperty>(Expression propertyPath)
			where TProperty : class;

		/// <summary>
		/// Není určeno pro přímé volání. Interní použití.
		/// </summary>
		IFluentDataLoaderAsync<TWrappedEntity> Unwrap<TWrappedEntity>()
			where TWrappedEntity : class;
	}
}