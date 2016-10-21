using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
	/// </summary>
	public interface IDataLoader
	{
		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
		void Load<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class;

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
		void LoadAll<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class;
	}
}