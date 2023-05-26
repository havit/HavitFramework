using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.States;
using Havit.Diagnostics.Contracts;
using Havit.Hangfire.Extensions.RecurringJobs.Services;

namespace Havit.Hangfire.Extensions.RecurringJobs;

/// <summary>
/// Recurring job to run another recurring jobs in sequence.
/// Why running recurring jobs and not a background jobs? Because running recurring jobs updates the state on the hangfire dashboard!
/// </summary>
public class SequenceRecurringJob : IRecurringJob
{
	/// <summary>
	/// Job identifier.
	/// </summary>
	public string JobId { get; }

	/// <summary>
	/// Queue name.
	/// </summary>
	public string Queue { get; }

	/// <summary>
	/// Cron expression.
	/// </summary>
	public string CronExpression { get; }

	/// <summary>
	/// Recurring jobs options.
	/// </summary>
	public RecurringJobOptions RecurringJobOptions { get; }

	/// <summary>
	/// RecurringJobs to run in sequence.
	/// </summary>
	public IRecurringJob[] RecurringJobsToRunInSequence { get; }

	/// <summary>
	/// Job continuation options (used to configure whether to continue when any job fails).
	/// </summary>
	public JobContinuationOptions JobContinuationOptions { get; }

	/// <summary>
	/// Constructor (for backward compatibility).
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SequenceRecurringJob(string jobId, string cronExpression, IRecurringJob[] recurringJobsToRunInSequence, JobContinuationOptions jobContinuationOptions = JobContinuationOptions.OnAnyFinishedState, TimeZoneInfo timeZone = null, string queue = EnqueuedState.DefaultQueue, MisfireHandlingMode misfireHandling = MisfireHandlingMode.Relaxed)
		: this(jobId, queue, cronExpression, new RecurringJobOptions { TimeZone = timeZone ?? TimeZoneInfo.Utc, MisfireHandling = misfireHandling }, recurringJobsToRunInSequence, jobContinuationOptions)
	{
		// NOOP
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public SequenceRecurringJob(string jobId, string queue, string cronExpression, RecurringJobOptions recurringJobOptions, IRecurringJob[] recurringJobsToRunInSequence, JobContinuationOptions jobContinuationOptions = JobContinuationOptions.OnAnyFinishedState)
	{
		Contract.Requires((recurringJobsToRunInSequence != null) && recurringJobsToRunInSequence.Any());

		this.JobId = jobId;
		this.Queue = queue;
		this.CronExpression = cronExpression;
		this.RecurringJobOptions = recurringJobOptions;
		this.RecurringJobsToRunInSequence = recurringJobsToRunInSequence;
		this.JobContinuationOptions = jobContinuationOptions;
	}

	/// <inheritdoc />
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		List<Exception> exceptions = new List<Exception>();

		foreach (var recurringJob in RecurringJobsToRunInSequence)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				await recurringJob.RunAsync(serviceProvider, cancellationToken);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				exceptions.Add(exception);
			}
		}

		if (exceptions.Any())
		{
			throw new AggregateException(exceptions);
		}
	}

	/// <inheritdoc />
	public void ScheduleAsRecurringJob(IRecurringJobManager recurringJobManager)
	{
		recurringJobManager.AddOrUpdate<ISequenceRecurringJobScheduler>(this.JobId, Queue, planner => planner.ProcessRecurryingJobsInQueue(JobId, RecurringJobsToRunInSequence.Select(item => item.JobId).ToArray(), this.JobContinuationOptions), CronExpression, RecurringJobOptions);
	}
}
