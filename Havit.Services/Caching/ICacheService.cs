namespace Havit.Services.Caching
{
	/// <summary>
	/// Interface abstrahující práci s cache.
	/// </summary>
	public interface ICacheService
	{
		/// <summary>
		/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
		/// </summary>
		bool SupportsCacheDependencies { get; }

		/// <summary>
		/// Přidá položku s daným klíčem a hodnotou do cache.
		/// </summary>
		void Add(string key, object value, CacheOptions options = null);

		/// <summary>
		/// Vyhledá položku s daným klíčem v cache.
		/// </summary>
		/// <returns>True, pokud položka v cache je, jinak false.</returns>
		bool TryGet(string key, out object result);

		/// <summary>
		/// Vrací true, pokud je položka s daným klíčem v cache.
		/// </summary>
		bool Contains(string key);

		/// <summary>
		/// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
		/// </summary>
		void Remove(string key);

		/// <summary>
		/// Vyčistí obsah cache.
		/// </summary>
		void Clear();
	}
}
