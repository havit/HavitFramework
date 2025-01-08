using Havit.Services.Caching;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.Caching.Hubs;

/// <summary>
/// Hub pro obsluhu zpráv pro distribuovanou invalidaci cache.
/// Určen pro použití v rámci webové aplikace běžící v jediné instanci.
/// </summary>
public class DistributedCacheInvalidationInAppHostedHub(
	ILogger<DistributedCacheInvalidationInAppHostedHub> _logger,
	[FromKeyedServices(DistributedCacheInvalidationCacheService.LocalCacheServiceKey)] ICacheService _localCacheService)
	: DistributedCacheInvalidationStandaloneHub(_logger)
{
	/// <inheritdoc />
	protected override async Task HandleClearMessageAsync()
	{
		await base.HandleClearMessageAsync();

		// vyčistíme lokální cache - díky tomu ve scénáři s jediným aplikačním serverem, který je též hubem, se nemusíme připojovat SignalR spojením "sami k sobě".
		_localCacheService.Clear();
	}

	/// <inheritdoc />
	protected override async Task HandleRemoveMessageAsync(string[] keysToRemove)
	{
		await base.HandleRemoveMessageAsync(keysToRemove);

		// odstraníme polžoky z lokální cache - díky tomu ve scénáři s jediným aplikačním serverem, který je též hubem, se nemusíme připojovat SignalR spojením "sami k sobě".
		_localCacheService.RemoveAll(keysToRemove);
	}

}