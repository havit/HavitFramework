using Hangfire;
using Hangfire.Storage;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <inheritdoc />
public class RecurringJobsScheduler : IRecurringJobsScheduler
{
	private readonly IRecurringJobManager _recurringJobManager;
	private readonly JobStorage _jobStorage;

	/// <summary>
	/// Constructor.
	/// </summary>
	public RecurringJobsScheduler(IRecurringJobManager recurringJobManager, JobStorage jobStorage)
	{
		this._recurringJobManager = recurringJobManager;
		this._jobStorage = jobStorage;
	}

	/// <inheritdoc />
	public void SetSchedule(params IRecurringJob[] recurringJobsToSchedule)
	{
		// schedule recurring jobs
		foreach (IRecurringJob job in recurringJobsToSchedule)
		{
			job.ScheduleAsRecurringJob(_recurringJobManager);
		}

		// Clear previous plan
		using (var connection = _jobStorage.GetConnection())
		{
			string[] jobsToRemove = connection.GetRecurringJobs().Select(item => item.Id).Except(recurringJobsToSchedule.Select(item => item.JobId)).ToArray();
			foreach (string jobId in jobsToRemove)
			{
				_recurringJobManager.RemoveIfExists(jobId);
			}
		}
	}
}
