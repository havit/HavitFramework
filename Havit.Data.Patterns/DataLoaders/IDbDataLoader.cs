using System.Collections.Generic;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Načítání lze zřetězit.
	/// <code>dbDataLoader.For(...).Load(item => item.Property1).Load(property1 => property1.Property2);</code>
	/// </summary>
	public interface IDbDataLoader
	{
		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> For<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> For<TEntity>(params TEntity[] entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> For<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> For<TEntity>(ICollection<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> For<TEntity>(List<TEntity> entities)
			where TEntity : class;
	}
}