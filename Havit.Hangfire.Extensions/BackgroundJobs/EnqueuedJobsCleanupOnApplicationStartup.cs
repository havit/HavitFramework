using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
internal class EnqueuedJobsCleanupOnApplicationStartup : IHostedService
{
	private readonly EnqueuedJobsCleanupOnApplicationStartupOptions _options;
	private readonly IBackgroundJobManager _backgroundJobManager;

	/// <summary>
	/// Constructor.
	/// </summary>
	public EnqueuedJobsCleanupOnApplicationStartup(IBackgroundJobManager backgroundJobManager, IOptions<EnqueuedJobsCleanupOnApplicationStartupOptions> options)
	{
		this._backgroundJobManager = backgroundJobManager;
		this._options = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		foreach (var queue in _options.Queues)
		{
			_backgroundJobManager.DeleteEnqueuedJobs(queue);
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