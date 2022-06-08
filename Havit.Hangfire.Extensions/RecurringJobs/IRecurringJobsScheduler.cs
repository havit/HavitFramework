namespace Havit.Hangfire.Extensions.RecurringJobs;

/// <summary>
/// Contains methods to schedule recurring jobs.
/// </summary>
public interface IRecurringJobsScheduler
{
	/// <summary>
	/// Schedules the recurring jobs and clears any additional already scheudled recurring jobs.
	/// </summary>
	void SetSchedule(params IRecurringJob[] recurringJobsToSchedule);
}