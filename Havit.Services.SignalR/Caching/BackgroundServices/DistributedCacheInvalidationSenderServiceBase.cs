using Havit.Services.SignalR.Caching.Internal;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Zajišťuje odeslání požadavku na invalidaci položek z cache do hubu.
/// </summary>
public abstract class DistributedCacheInvalidationSenderServiceBase(
	ILogger<DistributedCacheInvalidationSenderServiceBase> _logger) : IDistributedCacheInvalidationSenderService
{
	/// <summary>
	/// Zajišťuje odeslání požadavku na invalidaci položek z cache do hubu.
	/// </summary>
	public async Task InvalidateAsync(DistributedCacheInvalidationStorageDataSnapshot snapshot, CancellationToken cancellationToken)
	{
		if (snapshot.ClearCacheRequired)
		{
			try
			{
				_logger.LogDebug("Sending message to clear cache...");
				await ProcessClearCacheAsync(cancellationToken);
				_logger.LogDebug("Message to clear cache sent.");
			}
			catch (OperationCanceledException)
			{
				_logger.LogDebug("Sending message to clear cache cancelled.");
				throw;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to send message to clear cache.");
				throw;
			}
		}
		else if (snapshot.KeysToRemove.Count > 0)
		{
			try
			{
				// Maximální velikost zprávy SignalR spojení je 32K, vezmeme do velikosti stringů jen 80%, zbytek ponecháme na režii protokolu (JSON serializace).
				int maxLength = (int)Math.Floor(32_768 * 0.8M);

				foreach (string[] chunk in snapshot.KeysToRemove.ChunkifyStringsToMaxLength(maxLength))
				{
					_logger.LogDebug("Sending message to remove keys from cache...");
					await ProcessRemoveAsync(chunk, cancellationToken);
					_logger.LogDebug("Message to remove keys from cache sent.");
				}
			}
			catch (OperationCanceledException)
			{
				_logger.LogDebug("Sending message to remove keys from cache cancelled.");
				throw;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to send message to clear cache.");
				throw;
			}
		}
	}

	/// <summary>
	/// Zajišťuje odeslání požadavku na vyčištění cache do hubu.
	/// </summary>
	protected abstract Task ProcessClearCacheAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Zajišťuje odeslání požadavku na odstranění klíčů z cache do hubu.
	/// </summary>
	protected abstract Task ProcessRemoveAsync(string[] keysToRemove, CancellationToken cancellationToken);
}
