using Hangfire;
using Hangfire.Storage;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <inheritdoc />
public class RecurringJobsScheduler : IRecurringJobsScheduler
{
	private readonly IRecurringJobManager recurringJobManager;
	private readonly JobStorage jobStorage;

	/// <summary>
	/// Constructor.
	/// </summary>
	public RecurringJobsScheduler(IRecurringJobManager recurringJobManager, JobStorage jobStorage)
	{
		this.recurringJobManager = recurringJobManager;
		this.jobStorage = jobStorage;
	}

	/// <inheritdoc />
	public void SetSchedule(params IRecurringJob[] recurringJobsToSchedule)
	{
		// schedule recurring jobs
		foreach (IRecurringJob job in recurringJobsToSchedule)
		{
			job.ScheduleAsRecurringJob(recurringJobManager);
		}

		// Clear previous plan
		using (var connection = jobStorage.GetConnection())
		{
			string[] jobsToRemove = connection.GetRecurringJobs().Select(item => item.Id).Except(recurringJobsToSchedule.Select(item => item.JobId)).ToArray();
			foreach (string jobId in jobsToRemove)
			{
				recurringJobManager.RemoveIfExists(jobId);
			}
		}
	}
}
