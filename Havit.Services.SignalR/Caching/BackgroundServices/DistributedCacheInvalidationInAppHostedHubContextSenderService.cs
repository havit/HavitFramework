using Havit.Services.Caching;
using Havit.Services.SignalR.Caching.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Zajišťuje odeslání požadavku na invalidaci položek z cache do SignalR <see cref="DistributedCacheInvalidationInAppHostedHub"/>u pomocí <see cref="IHubContext{DistributedCacheInvalidationInAppHostedHub}"/>u.
/// </summary>
public class DistributedCacheInvalidationInAppHostedHubContextSenderService(
	ILogger<DistributedCacheInvalidationInAppHostedHubContextSenderService> _logger,
	IHubContext<DistributedCacheInvalidationInAppHostedHub> _hubContext) : DistributedCacheInvalidationSenderServiceBase(_logger)
{
	/// <inheritdoc />
	protected override async Task ProcessRemoveAsync(string[] keysToRemove, CancellationToken cancellationToken)
	{
		await _hubContext.Clients.All.SendAsync(nameof(ICacheService.Remove), keysToRemove, cancellationToken: cancellationToken);
	}

	/// <inheritdoc />
	protected override async Task ProcessClearCacheAsync(CancellationToken cancellationToken)
	{
		await _hubContext.Clients.All.SendAsync(nameof(ICacheService.Clear), cancellationToken: cancellationToken);
	}
}
