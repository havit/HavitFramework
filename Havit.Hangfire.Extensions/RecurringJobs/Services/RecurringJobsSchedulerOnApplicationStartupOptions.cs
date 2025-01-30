namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Options for RecurringJobsSchedulerOnApplicationStartup.
/// </summary>
internal class RecurringJobsSchedulerOnApplicationStartupOptions
{
	/// <summary>
	/// Recurring jobs to schedule.
	/// </summary>
	public List<IRecurringJob> RecurringJobs { get; } = new List<IRecurringJob>();
}
