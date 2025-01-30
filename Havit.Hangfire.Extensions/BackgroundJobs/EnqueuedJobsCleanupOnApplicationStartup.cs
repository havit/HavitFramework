using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
internal class EnqueuedJobsCleanupOnApplicationStartup : IHostedService
{
	private readonly EnqueuedJobsCleanupOnApplicationStartupOptions options;
	private readonly IBackgroundJobManager backgroundJobManager;

	/// <summary>
	/// Constructor.
	/// </summary>
	public EnqueuedJobsCleanupOnApplicationStartup(IBackgroundJobManager backgroundJobManager, IOptions<EnqueuedJobsCleanupOnApplicationStartupOptions> options)
	{
		this.backgroundJobManager = backgroundJobManager;
		this.options = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		foreach (var queue in options.Queues)
		{
			backgroundJobManager.DeleteEnqueuedJobs(queue);
		}
		return Task.CompletedTask;
	}

	public void StopApplication()
	{
		// NOOP
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// NOOP
		return Task.CompletedTask;
	}
}