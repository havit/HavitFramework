using System.Collections.Generic;
using Havit.Data.Patterns.DataLoaders;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny. Podporuje synchronní i asynchronní načítání. K metodám třídy se přistupuje přes interface <see cref="IDbDataLoader">IDbDataLoader</see> a <see cref="IDbDataLoaderAsync">IDbDataLoaderAsync</see>.
	/// Načítání lze zřetězit:
	/// <code>dbDataLoader.For(...).Load(item => item.Property1).Load(property1 => property1.Property2)</code>
	/// </summary>
	public class DbDataLoader : IDbDataLoader, IDbDataLoaderAsync
	{
		private readonly IDbContext dbContext;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		public DbDataLoader(IDbContext dbContext)
		{
			Contract.Requires(dbContext != null);

			this.dbContext = dbContext;
		}

		private DbDataLoaderFor<TEntity> ForInternal<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class
		{
			return new DbDataLoaderFor<TEntity>(dbContext, entities);
		}

		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(TEntity entity)
		{			
			return ForInternal(new TEntity[] { entity });
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(params TEntity[] entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(IEnumerable<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(ICollection<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(List<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(TEntity entity)
		{
			return ForInternal(new TEntity[] { entity });
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(params TEntity[] entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(IEnumerable<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(ICollection<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(List<TEntity> entities)
		{
			Contract.Requires(entities != null);

			return ForInternal(entities);
		}
	}
}
