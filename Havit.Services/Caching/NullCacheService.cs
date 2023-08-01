namespace Havit.Services.Caching;

/// <summary>
/// Implementace ICacheService, která nic nedělá - nekládá do žádného úložiště.
/// Nepodporuje cache dependencies.
/// </summary>
public class NullCacheService : ICacheService
{

	/// <summary>
	/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
	/// Tato třída vrací vždy false.
	/// </summary>
	public bool SupportsCacheDependencies
	{
		get
		{
			return false;
		}
	}

	/// <summary>
	/// Přidá položku s daným klíčem a hodnotou do cache. Tato třída nedělá nic.
	/// </summary>
	public void Add(string key, object value, CacheOptions options = null)
	{
		// NOOP
	}

	/// <summary>
	/// Vyhledá položku s daným klíčem v cache.
	/// Tato třída vrací vždy false.
	/// </summary>
	public bool TryGet(string key, out object result)
	{
		result = null;
		return false;
	}

	/// <summary>
	/// Vrací true, pokud je položka s daným klíčem v cache.
	/// Tato třída vrací vždy false.
	/// </summary>
	public bool Contains(string key)
	{
		return false;
	}

	/// <summary>
	/// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
	/// Tato třída nedělá nic.
	/// </summary>
	public void Remove(string key)
	{
		// NOOP
	}

	/// <summary>
	/// Vyčistí obsah cache.
	/// Tato třída nedělá nic.
	/// </summary>
	public void Clear()
	{
		// NOOP
	}
}