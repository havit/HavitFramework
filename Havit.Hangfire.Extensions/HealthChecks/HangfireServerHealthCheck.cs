#if NET8_0_OR_GREATER
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Havit.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Havit.Hangfire.Extensions.HealthChecks;

/// <summary>
/// Health check checking Hangfire server is running.
/// </summary>
/// <example>
/// Register with default options:
/// <code>services.AddHealthChecks().AddCheck&lt;HangfireServerHealthCheck&gt;("HangfireServer", timeout: defaultTimeout)</code>
/// Register with custom options:
/// <code>services.AddHealthChecks().AddTypeActivatedCheck&lt;HangfireServerHealthCheck&gt;("HangfireServer", new HangfireServerHealthCheckOptions { Queue = "myqueue", RequiredInstances = 2, MaxHeartbeatAgeSeconds = 180 });</code>
/// or when we want also set timeout (even the implementation does not use it currently):
/// <code>services.AddHealthChecks().AddTypeActivatedCheck&lt;HangfireServerHealthCheck&gt;("HangfireServer", failureStatus: HealthStatus.Unhealthy, tags: null, timeout: defaultTimeout, new HangfireServerHealthCheckOptions { Queue = "myqueue", RequiredInstances = 2, MaxHeartbeatAgeSeconds = 180 }); </code>
/// </example>
public class HangfireServerHealthCheck : IHealthCheck
{
	private readonly IMonitoringApi _monitoringApi;
	private readonly HangfireServerHealthCheckOptions _options;

	/// <summary>
	/// Constructor.
	/// </summary>
	public HangfireServerHealthCheck(JobStorage jobStorage, HangfireServerHealthCheckOptions options = null)
	{
		_monitoringApi = jobStorage.GetMonitoringApi();
		_options = options ?? new HangfireServerHealthCheckOptions();
	}

	async Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
	{
		try
		{
			return await this.CheckHealthAsync(cancellationToken);

		}
		catch (Exception exception)
		{
			return HealthCheckResult.Unhealthy(exception: exception);
		}
	}

	internal virtual DateTime UtcNow => DateTime.UtcNow;

	/// <summary>
	/// Checks the health of the Hangfire server.
	/// </summary>
	protected internal virtual Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
	{
		DateTime heartbeatTreshold = UtcNow - TimeSpan.FromSeconds(_options.MaxHeartbeatAgeSeconds);

		IList<ServerDto> servers = _monitoringApi.Servers();
		int liveServersCount = servers
			.WhereIf(!String.IsNullOrEmpty(_options.Queue), server => server.Queues.Contains(_options.Queue))
			.Where(server => server.Heartbeat >= heartbeatTreshold)
			.Count();

		string message = $"Running {liveServersCount} instance(s), required {_options.RequiredInstances} instances.";
		return (liveServersCount >= _options.RequiredInstances)
			? Task.FromResult(HealthCheckResult.Healthy(message))
			: Task.FromResult(HealthCheckResult.Unhealthy(message));
	}
}
#endif