using System.Linq.Expressions;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Fluent API pro data loader.
	/// Umožňuje zřetězený zápis pro načítání vlastností data loaderem.
	/// </summary>
	public interface IFluentDataLoader<out TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// Není určeno pro volání z klientského kódu.
		/// </summary>
		/// <param name="propertyPath">
		/// Vlastnost, která má být načtena.
		/// </param>
		/// <remarks>
		/// Aby fungovaly extension metody navěšené na IEnumerable&lt;IFluentDataLoader&gt;, je tento interface použit s <strong>out TEntity</strong>. Jenže pak nemůžeme v parametru metody použít Expression&lt;Func&lt;TEntity, TProperty&gt;&gt;, proto je zde jen Expression.
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IFluentDataLoader<TProperty> Load<TProperty>(Expression propertyPath)
			where TProperty : class;

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// Není určeno pro volání z klientského kódu.
		/// </summary>
		/// <param name="propertyPath">
		/// Vlastnost, která má být načtena.
		/// </param>
		Task<IFluentDataLoader<TProperty>> LoadAsync<TProperty>(Expression propertyPath)
			where TProperty : class;

		/// <summary>
		/// Není určeno pro přímé volání. Interní použití.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IFluentDataLoader<TWrappedEntity> Unwrap<TWrappedEntity>()
			where TWrappedEntity : class;
	}
}