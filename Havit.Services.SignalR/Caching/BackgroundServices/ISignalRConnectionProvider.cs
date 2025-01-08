using Microsoft.AspNetCore.SignalR.Client;

namespace Havit.Services.SignalR.Caching.BackgroundServices;

/// <summary>
/// Poskytuje SignalR spojení, zajišťuje, aby bylo spojení otevřené.
/// </summary>
public interface ISignalRConnectionProvider
{
	/// <summary>
	/// Poskytuje SignalR spojení, zajišťuje, aby bylo spojení otevřené.
	/// </summary>
	Task<HubConnection> GetConnectedHubConnectionAsync(CancellationToken cancellationToken);
}