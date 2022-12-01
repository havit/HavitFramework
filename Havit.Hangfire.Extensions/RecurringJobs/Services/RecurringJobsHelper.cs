using Hangfire;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Contains methods to schedule recurring jobs.
/// </summary>
public static class RecurringJobsHelper
{
	/// <summary>
	/// Schedules the recurring jobs and clears any additional already scheudled recurring jobs.
	/// </summary>
	public static void SetSchedule(params IRecurringJob[] recurringJobsToSchedule)
	{
		var recurringJobsHelperService = new RecurringJobsScheduler(new RecurringJobManager(JobStorage.Current), JobStorage.Current);
		recurringJobsHelperService.SetSchedule(recurringJobsToSchedule);
	}
}
