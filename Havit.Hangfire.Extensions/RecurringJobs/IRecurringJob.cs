using Hangfire;

namespace Havit.Hangfire.Extensions.RecurringJobs;

/// <summary>
/// Recurrying job to schedule.
/// </summary>
public interface IRecurringJob
{
	/// <summary>
	/// Job identifier.
	/// </summary>
	string JobId { get; }

	/// <summary>
	/// Schedules the job.
	/// </summary>
	void ScheduleAsRecurringJob(IRecurringJobManager recurringJobManager);

	/// <summary>
	/// Runs the job immediately.
	/// </summary>
	Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}