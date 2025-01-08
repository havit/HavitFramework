using Havit.Services.Caching;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// BackgroundService (IHostedService) která se připojí k SignalR hubu a poslouchá zprávy, jaké události si má z cache vyhodit.
/// </summary>
public class DistributedCacheInvalidationReceiverBackgroundService(
	ILogger<DistributedCacheInvalidationReceiverBackgroundService> _logger,
	IOptions<DistributedCacheInvalidationOptions> _options,
	[FromKeyedServices(DistributedCacheInvalidationCacheService.LocalCacheServiceKey)] ICacheService _localCacheService,
	ISignalRConnectionProvider _signalRConnectionProvider)
	: BackgroundService
{
	private readonly DistributedCacheInvalidationOptions _optionsValue = _options.Value;

	private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
	private HubConnection _connection;

	/// <inheritdoc />
	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		TaskCompletionSource taskCompletitionSource = new TaskCompletionSource();
		// v okamžiku cancellation requestu na cancellationTokenu, nastavíme taskCompletitionSource jako cancelovaný (propagujeme cancellation request na taskCompletionSource)
		using var cancellationTokenRegistration = cancellationToken.Register(() => { taskCompletitionSource.SetCanceled(cancellationToken); });

		// lokální funkce (pomáhá snadnému +=, -= na událost/delegáta)
		// lokální funkce je výhodná pro snadnou dostupnost taskCompletitionSource
		Task HandleConnectionClosedAsync(Exception _)
		{
			// V případě ukončení SignalR spojení cancelujeme taskCompletionSource.
			taskCompletitionSource.SetCanceled(CancellationToken.None);
			return Task.CompletedTask;
		}

		// navážeme spojení
		var connection = await _signalRConnectionProvider.GetConnectedHubConnectionAsync(cancellationToken);

		// Pokud navážeme spojení, vymažele lokální cache.
		// Pokud jsme již v cache měli něco, co jsme od ostatních nedostali z důvodu výpadku SignalR spojení,
		// preventivně vyhodíme z cache vše.
		if (_optionsValue.ClearLocalCacheWhenReceiverConnects)
		{
			_logger.LogInformation("Clearing local cache after connecting to SignalR hub...");
			_localCacheService.Clear();
			_logger.LogInformation("Local cache cleared.");
		};

		connection.Closed += HandleConnectionClosedAsync;

		// nastavíme zprávy, které chceme odebírat
		_logger.LogDebug("Attaching message handlers...");
		connection.On(nameof(ICacheService.Clear), new Type[0], HandleClearMessageAsync, null);
		connection.On<string[]>(nameof(ICacheService.Remove), HandleRemoveMessageAsync);
		_logger.LogDebug("Message handlers attached.");

		// zapamatujeme si nakonfigurované spojení
		_connection = connection;

		// spojení navázáno, čekáme
		// a) na požadavek na ukončení aplikaci
		// nebo
		// b) na uzavření SignalR spojení
		try
		{
			await taskCompletitionSource.Task;
		}
		catch (OperationCanceledException)
		{
			// došlo k požadavku na ukončení aplikace nebo na uzavření spojení
			_connection.Closed -= HandleConnectionClosedAsync;
			_connection.Remove(nameof(ICacheService.Clear));
			_connection.Remove(nameof(ICacheService.Remove));
			_connection = null;
		}
	}

	private Task HandleClearMessageAsync(object[] args, object state)
	{
		_logger.LogDebug("Received message to clear cache.");
		_localCacheService.Clear();
		return Task.CompletedTask;
	}

	private Task HandleRemoveMessageAsync(string[] keysToRemove)
	{
		_logger.LogDebug("Received message to remove keys from cache.");

		_localCacheService.RemoveAll(keysToRemove);
		return Task.CompletedTask;
	}
}
