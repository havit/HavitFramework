using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders.Fluent
{
	/// <summary>
	/// Rozšíření fluent API pro explicity data loader.
	/// Asynchronně načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Načítání lze (díky extension metodám v <see cref="TaskExtensions"/>) zřetězit.
	/// <code>await dataLoader.For(...).LoadAsync(item => item.Property1).LoadAsync(property1 => property1.Property2);</code>
	/// </summary>
	public interface IDataLoaderFluentAsync<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		IDataLoaderFluentAsync<TEntity> Where(Func<TEntity, bool> predicate);

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		Task<IDataLoaderFluentAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class;
	}
}