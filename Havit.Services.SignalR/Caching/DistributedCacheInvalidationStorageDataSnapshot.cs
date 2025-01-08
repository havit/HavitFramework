namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Reprezentuje data k realizaci distribuované invalidace cache.
/// </summary>
/// <remark>
/// Z logiky věci buď chceme smazat vše (<see cref="ClearCacheRequired"/>) NEBO máme individuální klíče k invalidaci (<see cref="KeysToRemove"/>) nebo ani jedno, ani druhé.
/// </remark>
public class DistributedCacheInvalidationStorageDataSnapshot
{
	/// <summary>
	/// Indikuje klíče, jež jsou požadovány na odstranění z cache.
	/// </summary>
	public HashSet<string> KeysToRemove { get; } = new HashSet<string>();

	/// <summary>
	/// Indikuje, zda je požadováno vyčištění veškerého obsahu cache.
	/// </summary>
	public bool ClearCacheRequired { get; set; }

	/// <summary>
	/// Indikuje, zda je snapshot prázdný (nepředstavuje žádný požadavek na komunikaci mezi systémy).
	/// </summary>
	public bool IsEmpty()
	{
		return !ClearCacheRequired && (KeysToRemove.Count == 0);
	}
}