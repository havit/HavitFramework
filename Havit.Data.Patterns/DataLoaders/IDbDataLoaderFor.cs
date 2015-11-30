using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Načítání lze zřetězit.
	/// <code>dbDataLoader.For(...).Load(item => item.Property1).Load(property1 => property1.Property2);</code>
	/// </summary>
	public interface IDbDataLoaderFor<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dbDataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		IDbDataLoaderFor<TEntity> Where(Func<TEntity, bool> predicate);

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class;

		/// <summary>
		/// Načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TProperty : class;

		/// <summary>
		/// Načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TProperty : class;

		/// <summary>
		/// Načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TProperty : class;
	}
}