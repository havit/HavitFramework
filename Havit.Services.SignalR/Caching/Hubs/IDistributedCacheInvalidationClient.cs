using Havit.Services.Caching;
using Microsoft.AspNetCore.SignalR;

namespace Havit.Services.SignalR.Caching.Hubs;

/// <summary>
/// Reprezentuje zprávy, které umí obsloužit klienti SignalR hubu pro distribuovanou invalidaci cache.
/// </summary>
public interface IDistributedCacheInvalidationClient
{
	/// <summary>
	/// Zpráva pro invalidaci veškerého obsahu cache.
	/// </summary>
	[HubMethodName(nameof(ICacheService.Clear))]
	Task ClearAsync();

	/// <summary>
	/// Zpráva pro invalidaci vybraných klíčů v cache.
	/// </summary>
	[HubMethodName(nameof(ICacheService.Remove))]
	Task RemoveAsync(string[] keysToRemove);
}