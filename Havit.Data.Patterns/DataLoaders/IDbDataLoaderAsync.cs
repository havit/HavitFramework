using System.Collections.Generic;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Asynchronně načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Načítání lze (díky extension metodám v <see cref="TaskExtensions"/>) zřetězit.
	/// <code>await dbDataLoader.For(...).LoadAsync(item => item.Property1).LoadAsync(property1 => property1.Property2);</code>
	/// </summary>
	public interface IDbDataLoaderAsync
	{
		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> For<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> For<TEntity>(params TEntity[] entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> For<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> For<TEntity>(ICollection<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> For<TEntity>(List<TEntity> entities)
			where TEntity : class;
	}
}