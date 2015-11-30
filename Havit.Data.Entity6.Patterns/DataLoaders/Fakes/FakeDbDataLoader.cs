using System.Collections.Generic;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.Entity.Patterns.DataLoaders.Fakes
{
	/// <summary>
	/// Prázdná implementace interface <see cref="IDbDataLoader" /> a <see cref="IDbDataLoaderAsync" />, která efektivně nevykonává žádnou činnost.
	/// Určeno pro použití v unit testech.
	/// </summary>
	[Fake]	
	public class FakeDbDataLoader : IDbDataLoader, IDbDataLoaderAsync
	{
		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(TEntity entity)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(params TEntity[] entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(IEnumerable<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(ICollection<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou dataloaderem načítány.
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány.</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(List<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekt, jehož vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderAsync.For<TEntity>(TEntity entity)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(params TEntity[] entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(IEnumerable<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(ICollection<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Přijímá objekty, jejichž vlastnosti budou načítány (asynchronně).
		/// </summary>
		/// <param name="entities">Objekty, jejichž vlastnosti budou dataloaderem načítány (asynchronně).</param>
		IDbDataLoaderFor<TEntity> IDbDataLoader.For<TEntity>(List<TEntity> entities)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}
	}
}