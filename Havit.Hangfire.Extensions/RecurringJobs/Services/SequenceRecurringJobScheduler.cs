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

		string jobId = recurringJobManager.TriggerExecution(recurringJobIdToEnqueue);
		Contract.Assert(jobId != null);

		var nextRecurringJobIdsToRunInSequence = remainingRecurringJobIdsToRunInSequence.Skip(1).ToArray();
		if (nextRecurringJobIdsToRunInSequence.Any())
		{
			logger.LogDebug("Enqueueing continuation to run next {Count} jobs in the sequence '{SequnceRecurringJobId}'.", nextRecurringJobIdsToRunInSequence.Length, sequenceRecurringJobId);

			// parameters are serialized!
			backgroundJobClient.ContinueJobWith(jobId, () => EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions), jobContinuationOptions);
		}
	}
}