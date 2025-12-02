#if NET8_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
internal class RecurringJobsSchedulerOnApplicationStartup : IHostedService
{
	private readonly IRecurringJobsScheduler _recurringJobsScheduler;
	private readonly RecurringJobsSchedulerOnApplicationStartupOptions _options;

	/// <summary>
	/// Constructor.
	/// </summary>
	public RecurringJobsSchedulerOnApplicationStartup(IRecurringJobsScheduler recurringJobsHelperService, IOptions<RecurringJobsSchedulerOnApplicationStartupOptions> options)
	{
		_recurringJobsScheduler = recurringJobsHelperService;
		this._options = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_recurringJobsScheduler.SetSchedule(_options.RecurringJobs.ToArray());
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
#endif