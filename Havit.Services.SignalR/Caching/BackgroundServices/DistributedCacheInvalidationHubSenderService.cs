using Havit.Services.Caching;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Zajišťuje odeslání požadavku na invalidaci položek z cache do pomocí SignalR.
/// </summary>
public class DistributedCacheInvalidationHubSenderService(
	ILogger<DistributedCacheInvalidationHubSenderService> _logger,
	ISignalRConnectionProvider _signalRConnectionProvider) : DistributedCacheInvalidationSenderServiceBase(_logger)
{
	/// <inheritdoc />
	protected override async Task ProcessRemoveAsync(string[] keysToRemove, CancellationToken cancellationToken)
	{
		var connection = await _signalRConnectionProvider.GetConnectedHubConnectionAsync(cancellationToken);
		await connection.SendAsync(nameof(ICacheService.Remove), keysToRemove, cancellationToken: cancellationToken);
	}

	/// <inheritdoc />
	protected override async Task ProcessClearCacheAsync(CancellationToken cancellationToken)
	{
		var connection = await _signalRConnectionProvider.GetConnectedHubConnectionAsync(cancellationToken);
		await connection.SendAsync(nameof(ICacheService.Clear), cancellationToken: cancellationToken);
	}
}
