using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Poskytuje SignalR spojení, zajišťuje, aby bylo spojení otevřené.
/// </summary>
public class SignalRConnectionProvider(
	ILogger<SignalRConnectionProvider> _logger, IOptions<DistributedCacheInvalidationOptions> _options) : ISignalRConnectionProvider, IAsyncDisposable
{
	private DistributedCacheInvalidationOptions _optionsValue = _options.Value;

	private HubConnection _connection;
	private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

	/// <summary>
	/// Poskytuje SignalR spojení, zajišťuje, aby bylo spojení otevřené.
	/// </summary>
	public async Task<HubConnection> GetConnectedHubConnectionAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("Waiting for lock...");
		await _semaphore.WaitAsync(cancellationToken);
		_logger.LogDebug("Lock acquired.");

		try
		{
			EnsureConnection();

			// Pokud je již spojení navazováno, nemáme lepší způsob, jak počkat na navázání spojení.
			if (_connection.State == HubConnectionState.Connecting)
			{
				_logger.LogDebug("Waiting to connect to SignalR hub...");
				while (_connection.State == HubConnectionState.Connecting)
				{
					await Task.Delay(100, cancellationToken); // Logovat výjimku v případě neúspěchu (cancellation token) asi nemá smysl.
				}
				if (_connection.State == HubConnectionState.Connected)
				{
					_logger.LogDebug("Connected to SignalR hub.");
				}
			}

			// Pokud je již spojení obnovováno, počkáme, jak to dopadne - zda se reconnect podaří.
			if (_connection.State == HubConnectionState.Reconnecting)
			{
				_logger.LogDebug("Waiting to reconnect...");
				var tcs = new TaskCompletionSource<int>();

				Task HandleConnectionClosedAsync(Exception _) { tcs.SetCanceled(CancellationToken.None); return Task.CompletedTask; }
				Task HandleConnectionReconnectedAsync(string _) { tcs.SetResult(0); return Task.CompletedTask; }

				_connection.Closed += HandleConnectionClosedAsync;
				_connection.Reconnected += HandleConnectionReconnectedAsync;

				try
				{
					await tcs.Task; // počkáme, než nám Reconnected signalizuje navázání spojení, ev. Closed hlásí neúspěch (výjimka).

					if (_connection.State == HubConnectionState.Connected)
					{
						_logger.LogDebug("Connected to SignalR hub.");
					}
				}
				catch (OperationCanceledException)
				{
					_logger.LogDebug("Connection to SignalR hub cancelled.");

					// NOOP.
					// Spojení se nepodařilo navázat, tj. dostali jsme jej do stavu Disconnected
					// Pokračujeme dále novým navázání spojení.
				}
				finally
				{
					_connection.Closed -= HandleConnectionClosedAsync;
					_connection.Reconnected -= HandleConnectionReconnectedAsync;
				}
			}

			var connectionRetryDelays = new ConnectionRetryDelays();
			while (_connection.State == HubConnectionState.Disconnected)
			{
				try
				{
					_logger.LogInformation("Connecting to SignalR hub...");
					await _connection.StartAsync(cancellationToken);
					_logger.LogInformation("Connected to SignalR hub.");
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Connection to SignalR hub failed.");

					int delay = connectionRetryDelays.GetNextDelay();
					if (delay > 0)
					{
						_logger.LogDebug("Waiting {ms} ms to retry...", delay);
						await Task.Delay(delay, cancellationToken);
						_logger.LogDebug("Waiting to retry finished.");
					}
				}
			}
		}
		finally
		{
			_logger.LogDebug("Releasing lock...");
			_semaphore.Release();
			_logger.LogDebug("Lock released.");
		}

		Havit.Diagnostics.Contracts.Contract.Assert(_connection.State == HubConnectionState.Connected);

		return _connection;
	}

	private void EnsureConnection()
	{
		if (_connection == null)
		{
			_connection = new HubConnectionBuilder()
				.WithUrl(new Uri(_optionsValue.HubUrl), _optionsValue.ConfigureHttpConnection)
				.WithAutomaticReconnect()
				.Build();
			_logger.LogDebug("Connection created (not started/opened).");
		}
		else
		{
			_logger.LogDebug("Reused existing connection.");
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (_connection != null)
		{
			await _connection.DisposeAsync();
			_connection = null;
		}
	}

	private class ConnectionRetryDelays
	{
		private int _current = 0;
		private Queue<int> _queue = new Queue<int>([0, 2_000, 10_000, 30_000]);

		public int GetNextDelay()
		{
			if (_queue.Count > 0)
			{
				_current = _queue.Dequeue();
			}

			return _current;
		}
	}
}