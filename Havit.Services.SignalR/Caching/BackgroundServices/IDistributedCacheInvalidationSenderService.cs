namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Zajišťuje odeslání požadavku na invalidaci položek z cache do SignalR hubu.
/// </summary>
public interface IDistributedCacheInvalidationSenderService
{
	/// <summary>
	/// Zajišťuje odeslání požadavku na invalidaci položek z cache do SignalR hubu.
	/// </summary>
	Task InvalidateAsync(DistributedCacheInvalidationStorageDataSnapshot snapshot, CancellationToken stoppingToken);
}
