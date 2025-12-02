#if NET8_0_OR_GREATER
namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Options for EnqueuedJobsCleanupOnApplicationStartup.
/// </summary>
internal class EnqueuedJobsCleanupOnApplicationStartupOptions
{
	/// <summary>
	/// Queues to clean.
	/// </summary>
	public List<string> Queues { get; } = new List<string>();
}
#endif