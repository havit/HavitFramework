namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Zajišťuje evidenci požadavků na distribuovanou invalidaci cache.
/// </summary>
public class DistributedCacheInvalidationStorageService : IDistributedCacheInvalidationStorageService
{
	private DistributedCacheInvalidationStorageDataSnapshot _dataSnapshot = new DistributedCacheInvalidationStorageDataSnapshot();
	private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

	/// <inheritdoc />
	public void AddKeyToRemove(string keyToRemove)
	{
		_semaphore.Wait();
		try
		{
			// pokud je požádáno o clear cache, tak již nepotřebujeme klíče, které mají být vyhozeny - vyhozeno bude "vše".
			if (!_dataSnapshot.ClearCacheRequired)
			{
				_dataSnapshot.KeysToRemove.Add(keyToRemove);
			}
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <inheritdoc />
	public void SetClearCacheRequired()
	{
		_semaphore.Wait();
		try
		{
			_dataSnapshot.ClearCacheRequired = true;
			// pokud je požádáno o clear cache, tak již nepotřebujeme klíče, které mají být vyhozeny - vyhozeno bude "vše".
			_dataSnapshot.KeysToRemove.Clear();
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <inheritdoc />
	public DistributedCacheInvalidationStorageDataSnapshot GetDataSnapshot()
	{
		_semaphore.Wait();
		try
		{
			var currentDataSnapshot = _dataSnapshot;
			_dataSnapshot = new DistributedCacheInvalidationStorageDataSnapshot();
			return currentDataSnapshot;
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <inheritdoc />
	public void AddSnapshot(DistributedCacheInvalidationStorageDataSnapshot snapshot)
	{
		_semaphore.Wait();
		try
		{
			if (snapshot.ClearCacheRequired)
			{
				_dataSnapshot.ClearCacheRequired = true;
			}
			else if (snapshot.KeysToRemove.Count > 0)
			{
				_dataSnapshot.KeysToRemove.UnionWith(snapshot.KeysToRemove);
			}
		}
		finally
		{
			_semaphore.Release();
		}
	}
}
