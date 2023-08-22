using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders;

/// <summary>
/// Extension metody k data loaderu pro zpětnou kompatibilitu - použití params u propertyPaths (bez CancellationTokenu).
/// (Klíčové slovo "params" bylo z asynchronních metod IDataLoaderu odstraněno pro možnost zadání CancellationTokenu nako posledního argumentu.)
/// </summary>
public static class DataLoaderExtensions
{
	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	public static async Task LoadAsync<TEntity>(this IDataLoader dataLoader, TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths /* no cancellation token here */)
		where TEntity : class
	{
		await dataLoader.LoadAsync(entity, propertyPaths, CancellationToken.None).ConfigureAwait(false);
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	public static async Task LoadAllAsync<TEntity>(this IDataLoader dataLoader, IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths /* no cancellation token here */)
		where TEntity : class
	{
		await dataLoader.LoadAllAsync(entities, propertyPaths, CancellationToken.None).ConfigureAwait(false);
	}
}