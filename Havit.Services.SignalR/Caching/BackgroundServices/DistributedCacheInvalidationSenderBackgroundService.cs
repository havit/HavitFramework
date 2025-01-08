using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// BackgroundService (IHostedService) která zajišťuje zpracování (rozeslání) zpráv k distribuované invalidaci cache.
/// </summary>
public class DistributedCacheInvalidationSenderBackgroundService(
	ILogger<DistributedCacheInvalidationSenderBackgroundService> _logger,
	IOptions<DistributedCacheInvalidationOptions> _options,
	IDistributedCacheInvalidationStorageService _distributedInvalidationStorageService,
	IDistributedCacheInvalidationSenderService _distributedInvalidationSenderService)
	: BackgroundService
{
	private DistributedCacheInvalidationOptions _optionsValue = _options.Value;

	/// <inheritdoc />
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			// TODO JK: Odstranit náhradou za WaitForDataSnapshot(_optionsValue.SenderWaitTimeToBufferMessages, cancellationToken);

			try
			{
				await Task.Delay(_optionsValue.SenderWaitTimeToBufferMessages, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				break;
			}

			var snapshot = _distributedInvalidationStorageService.GetDataSnapshot();
			try
			{
				await _distributedInvalidationSenderService.InvalidateAsync(snapshot, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				_distributedInvalidationStorageService.AddSnapshot(snapshot);
			}
		}
	}
}