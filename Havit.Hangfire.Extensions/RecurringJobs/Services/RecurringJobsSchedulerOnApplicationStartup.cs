using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
internal class RecurringJobsSchedulerOnApplicationStartup : IHostedService
{
	private readonly IRecurringJobsScheduler recurringJobsScheduler;
	private readonly RecurringJobsSchedulerOnApplicationStartupOptions options;

	/// <summary>
	/// Constructor.
	/// </summary>
	public RecurringJobsSchedulerOnApplicationStartup(IRecurringJobsScheduler recurringJobsHelperService, IOptions<RecurringJobsSchedulerOnApplicationStartupOptions> options)
	{
		recurringJobsScheduler = recurringJobsHelperService;
		this.options = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		recurringJobsScheduler.SetSchedule(options.RecurringJobs.ToArray());
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
