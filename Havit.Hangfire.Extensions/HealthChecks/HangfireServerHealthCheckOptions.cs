#if NET8_0_OR_GREATER
namespace Havit.Hangfire.Extensions.HealthChecks;

/// <summary>
/// Hangfire server health check options.
/// </summary>
public class HangfireServerHealthCheckOptions
{
	/// <summary>
	/// Quene name to check. If null or empty, all queues are considered.
	/// Default value is null;
	/// </summary>
	public string Queue { get; set; }

	/// <summary>
	/// Number of required live Hangfire server instances.
	/// Default value is 1.
	/// </summary>
	public int RequiredInstances { get; set; } = 1;

	/// <summary>
	/// Maximum age of server heartbeats to consider a server alive.
	/// Default value is 300 seconds (5 minutes).
	/// </summary>
	public int MaxHeartbeatAgeSeconds { get; set; } = 300;
}
#endif