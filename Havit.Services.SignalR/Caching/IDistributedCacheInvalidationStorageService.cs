namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Sbírá a eviduje požadavky na distribuovanou invalidaci cache a poskytuje metody pro práci s těmito daty.
/// </summary>
public interface IDistributedCacheInvalidationStorageService
{
	/// <summary>
	/// Zaeviduje klíč do cache k distrubované invalidaci.
	/// </summary>
	void AddKeyToRemove(string keyToRemove);

	/// <summary>
	/// Zaregistruje pořadavek na úplné promazání cache.
	/// </summary>
	void SetClearCacheRequired();

	/// <summary>
	/// Vrátí aktuální evidovaná data k distribuované invalidaci.
	/// </summary>
	DistributedCacheInvalidationStorageDataSnapshot GetDataSnapshot();

	/// <summary>
	/// Vloží data snapshotu mezi data k distribuované invalidaci.
	/// Slouží k "vrácení dat" v případě neúspěchu odeslání k invalidaci.
	/// </summary>
	void AddSnapshot(DistributedCacheInvalidationStorageDataSnapshot snapshot);
}
