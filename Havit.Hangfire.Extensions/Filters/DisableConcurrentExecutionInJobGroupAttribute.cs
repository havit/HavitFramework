using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Havit.Diagnostics.Contracts;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Ensures jobs in the same group cannot are not run concurrently.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
public class DisableConcurrentExecutionInJobGroupAttribute : JobFilterAttribute, IServerFilter
{
	private const string ProcessingJobIdKeyName = "ProcessingJobId";

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="jobGroupName">Job group name. Jobs in the same group cannot be run concurrently.</param>
	public DisableConcurrentExecutionInJobGroupAttribute(string jobGroupName)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(jobGroupName), nameof(jobGroupName));
		JobGroupName = jobGroupName;
		Order = 10;
	}

	/// <summary>
	/// Job group name. Jobs in the same group cannot be run concurrently.
	/// </summary>
	public string JobGroupName { get; set; }

	private string DistributedLockName => nameof(DisableConcurrentExecutionInJobGroupAttribute) + "_" + JobGroupName;

	private string HashName => $"disable-concurrent-execution-in-job-group:{JobGroupName}";

	private TimeSpan AcquireDistributedLockTimeout => TimeSpan.FromSeconds(15);

	/// <inheritdoc />
	public void OnPerforming(PerformingContext filterContext)
	{
		// we want to make read-write run as a critical section so we use a distributed lock (read = GetAllEntriesFromHash, write = SetRangeInHash).
		using (filterContext.Connection.AcquireDistributedLock(DistributedLockName, AcquireDistributedLockTimeout))
		{
			var hashEntries = filterContext.Connection.GetAllEntriesFromHash(HashName);

			// if we have a processing job id
			if ((hashEntries != null) && hashEntries.TryGetValue(ProcessingJobIdKeyName, out string processingJobId) && !String.IsNullOrEmpty(processingJobId))
			{
				// and the job is still processing (to make it safe when the server with a running job is killed)
				JobData jobData = filterContext.Connection.GetJobData(processingJobId);
				if (jobData.State == ProcessingState.StateName)
				{
					// we do not allow to run this job in the same group
					filterContext.Canceled = true;
					// and change the state of the job to the "AwaitingState"
					new BackgroundJobStateChanger().ChangeState(new StateChangeContext(filterContext.Storage, filterContext.Connection, filterContext.BackgroundJob.Id, new AwaitingState(processingJobId) { Reason = $"Disabled concurrent execution in a job group {JobGroupName}." }));
					return;
				}
			}

			// we did not find currently procesing job in the same job group
			// Let's process the job!
			// But note first the job in the job group is currently running.
			filterContext.Connection.SetRangeInHash(HashName, new Dictionary<string, string>
			{
				{ ProcessingJobIdKeyName, filterContext.BackgroundJob.Id }
			});
		}
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext filterContext)
	{
		// Clear the information about currently running job.
		using (filterContext.Connection.AcquireDistributedLock(DistributedLockName, AcquireDistributedLockTimeout))
		{
			filterContext.Connection.SetRangeInHash(HashName, new Dictionary<string, string>
			{
				{ ProcessingJobIdKeyName, String.Empty }
			});
		}
	}
}
