﻿namespace Havit.Services.Caching;

/// <summary>
/// Extension metody ke ICacheService.
/// </summary>
public static class CacheServiceExtensions
{
	/// <summary>
	/// Vyhledá položku s daným klíčem v cache.
	/// </summary>
	/// <returns>True, pokud položka v cache je, jinak false.</returns>
	/// <exception cref="InvalidOperationException">Hodnotu nalezenou v cache se nepodařilo zkonvertovat na cílový typ.</exception>
	public static bool TryGet<T>(this ICacheService cacheService, string cacheKey, out T result)
	{
		if (cacheService.TryGet(cacheKey, out object cacheValue))
		{
			try
			{
				result = (T)cacheValue;
			}
			catch (InvalidCastException exception)
			{
				throw new InvalidOperationException($"Value was found in the cache for key '{cacheKey}' but it was not possible to cast it to {typeof(T).FullName}.", exception);
			}
			return true;
		}
		else
		{
			result = default(T);
			return false;
		}
	}

	/// <summary>
	/// Odstraní z cache položky pro všechny předané klíče.
	/// Až budeme chtít umět předávat všechny klíče naráz (pro performance) pro invalidaci distribuované cache, přesuneme tuto metodu do ICacheService.
	/// </summary>
	public static void RemoveAll(this ICacheService cacheService, IEnumerable<string> cacheKeys)
	{
		foreach (string cacheKey in cacheKeys)
		{
			cacheService.Remove(cacheKey);
		}
	}
}
