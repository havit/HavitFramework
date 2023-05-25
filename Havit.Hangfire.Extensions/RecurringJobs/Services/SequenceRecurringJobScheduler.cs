using System.Linq;
using Hangfire;
using Havit.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <inheritdoc />
public class SequenceRecurringJobScheduler : ISequenceRecurringJobScheduler
{
	private readonly RecurringJobManager recurringJobManager; // IRecurryingJobManager nemá TriggerExection, jenž vrací jobId
	private readonly ILogger<SequenceRecurringJobScheduler> logger;
	private readonly IBackgroundJobClient backgroundJobClient;

	/// <summary>
	/// Constructor.
	/// </summary>
	public SequenceRecurringJobScheduler(ILogger<SequenceRecurringJobScheduler> logger, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
	{
		Contract.Requires(recurringJobManager is RecurringJobManager);

		this.logger = logger;
		this.recurringJobManager = (RecurringJobManager)recurringJobManager;
		this.backgroundJobClient = backgroundJobClient;
	}

	/// <inheritdoc />
	public void ProcessRecurryingJobsInQueue(string sequenceRecurringJobId, string[] recurringJobIdsToRunInSequence, JobContinuationOptions jobContinuationOptions)
	{
		EnqueueNextRecurringJob(sequenceRecurringJobId, null, recurringJobIdsToRunInSequence, jobContinuationOptions);
	}

	/// <summary>
	/// Enqueues next recurring job from <c>remainingRecurringJobIdsToRunInSequence</c> and waits enqueueing next jobs until its completion.
	/// </summary>
	/// <param name="sequenceRecurringJobId">Job (RecurringJobId) sequence identifier. For logging purposes only.</param>
	/// <param name="previousSequenceRecurringJobId">Previously scheduled recurring job identifier. For logging purposes only.</param>
	/// <param name="remainingRecurringJobIdsToRunInSequence">Remaining recurring job to schedule.</param>
	/// <param name="jobContinuationOptions">Job continuation options.</param>
	public void EnqueueNextRecurringJob( // public: required for backgroundJobClient.ContinueJobWith(...)
		/* Be careful: The arguments are used also in JobNameHelper to format job name! */
		string sequenceRecurringJobId,
		string previousSequenceRecurringJobId,
		string[] remainingRecurringJobIdsToRunInSequence,
		JobContinuationOptions jobContinuationOptions)
	{
		string recurringJobIdToEnqueue = remainingRecurringJobIdsToRunInSequence.First();

		if (previousSequenceRecurringJobId == null)
		{
			logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a first job in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, sequenceRecurringJobId);
		}
		else
		{
			logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a continuation of job '{PreviousRecurringJobId}' in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, previousSequenceRecurringJobId, sequenceRecurringJobId);
		}

		string jobId = recurringJobManager.TriggerJob(recurringJobIdToEnqueue);
		if (jobId == null)
		{
			logger.LogWarning("Triggering recurring job '{RecurringJobId}' failed.", recurringJobIdToEnqueue);
			if (jobContinuationOptions == JobContinuationOptions.OnlyOnSucceededState)
			{
				// job cheme označit za selhaný
				throw new TriggeringNextJobFailedException($"Triggering next jobs stopped ({recurringJobIdToEnqueue} was not successfully triggered).");
			}
		}

		var nextRecurringJobIdsToRunInSequence = remainingRecurringJobIdsToRunInSequence.Skip(1).ToArray();
		if (nextRecurringJobIdsToRunInSequence.Any())
		{
			if (jobId == null)
			{
				logger.LogDebug("Continuing with next jobs...");
				// pokud naplánování spuštění dané naplánované úlohy selhalo, a můžeme pokračovat i v případě neúspěchu,
				// pokračujeme v naplánování dalšího kroku
				EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions);
			}
			else
			{
				logger.LogDebug("Enqueueing continuation to run next {Count} jobs in the sequence '{SequnceRecurringJobId}'.", nextRecurringJobIdsToRunInSequence.Length, sequenceRecurringJobId);

				// parameters are serialized!
				backgroundJobClient.ContinueJobWith(jobId, () => EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions), jobContinuationOptions);
			}
		}
	}

	#region TriggeringNextJobFailedException  (nested class)
	/// <summary>
	/// Exception throw when triggering job failed.
	/// </summary>
	public class TriggeringNextJobFailedException : System.Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TriggeringNextJobFailedException(string message) : base(message)
		{
			// NOOP
		}
	}
	#endregion
}