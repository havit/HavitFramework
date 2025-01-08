using Havit.Services.Caching;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.Caching.Hubs;

/// <summary>
/// Hub pro obsluhu zpráv pro distribuovanou invalidaci cache.
/// Určen pro použití v samostatné webové aplikaci (mimo server s aplikační logikou).
/// </summary>
public class DistributedCacheInvalidationStandaloneHub(
	ILogger<DistributedCacheInvalidationStandaloneHub> _logger)
	: Hub<IDistributedCacheInvalidationClient>
{
	/// <summary>
	/// Obsluha zprávy pro invalidaci veškerého obsahu cache.
	/// </summary>
	[HubMethodName(nameof(ICacheService.Clear))]
	public async Task ClearAsync()
	{
		try
		{
			_logger.LogDebug("Handling message to clear cache...");
			await HandleClearMessageAsync();
			_logger.LogDebug("Message to clear cache handled.");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Handling message to clear cache failed.");
			throw;
		}
	}

	/// <summary>
	/// Obsluha zprávy pro invalidaci veškerého obsahu cache.
	/// </summary>
	protected virtual async Task HandleClearMessageAsync()
	{
		// pošleme zprávu ostatním příjemcům
		await Clients.Others.ClearAsync();
	}

	/// <summary>
	/// Obsluha zprávy pro invalidaci vybraných klíčů v cache.
	/// </summary>
	[HubMethodName(nameof(ICacheService.Remove))]
	public async Task RemoveAsync(string[] keysToRemove)
	{
		try
		{
			_logger.LogDebug("Handling message to remove keys from cache...");
			await HandleRemoveMessageAsync(keysToRemove);
			_logger.LogDebug("Message to remove keys from cache handled.");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Handling message to remove keys from cache failed.");
			throw;
		}
	}

	/// <summary>
	/// Obsluha zprávy pro invalidaci vybraných klíčů v cache.
	/// </summary>
	protected virtual async Task HandleRemoveMessageAsync(string[] keysToRemove)
	{
		// pošleme zprávu ostatním příjemcům
		await Clients.Others.RemoveAsync(keysToRemove);
	}

}