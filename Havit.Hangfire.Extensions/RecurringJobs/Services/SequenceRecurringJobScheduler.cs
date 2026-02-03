using Hangfire;
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Logging;
#endif

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Ensures running recurrying jobs sequence. Used for <see cref="SequenceRecurringJob" />
/// </summary>
public class SequenceRecurringJobScheduler
{
#if NET8_0_OR_GREATER
	private readonly ILogger<SequenceRecurringJobScheduler> _logger;
#endif
	private readonly IRecurringJobManagerV2 _recurringJobManager;
	private readonly IBackgroundJobClient _backgroundJobClient;

	/// <summary>
	/// Constructor.
	/// </summary>
	public SequenceRecurringJobScheduler(
#if NET8_0_OR_GREATER
		ILogger<SequenceRecurringJobScheduler> logger,
#endif
		IRecurringJobManagerV2 recurringJobManager,
		IBackgroundJobClient backgroundJobClient
		)
	{
#if NET8_0_OR_GREATER
		this._logger = logger;
#endif
		this._recurringJobManager = recurringJobManager;
		this._backgroundJobClient = backgroundJobClient;
	}

	/// <summary>
	/// Ensures running recurrying jobs sequence. 
	/// </summary>
	public void ProcessRecurryingJobsInQueue(
		/* Be careful: The arguments are used also in JobNameHelper to format job name! */
		string sequenceRecurringJobId,
		string[] recurringJobIdsToRunInSequence,
		JobContinuationOptions jobContinuationOptions)
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

#if NET8_0_OR_GREATER
		if (previousSequenceRecurringJobId == null)
		{
			_logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a first job in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, sequenceRecurringJobId);
		}
		else
		{
			_logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a continuation of job '{PreviousRecurringJobId}' in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, previousSequenceRecurringJobId, sequenceRecurringJobId);
		}
#endif

		string jobId = _recurringJobManager.TriggerJob(recurringJobIdToEnqueue);
		if (jobId == null)
		{
#if NET8_0_OR_GREATER
			_logger.LogWarning("Triggering recurring job '{RecurringJobId}' failed.", recurringJobIdToEnqueue);
#endif
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
#if NET8_0_OR_GREATER
				_logger.LogDebug("Continuing with next jobs...");
#endif
				// pokud naplánování spuštění dané naplánované úlohy selhalo, a můžeme pokračovat i v případě neúspěchu,
				// pokračujeme v naplánování dalšího kroku
				EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions);
			}
			else
			{
#if NET8_0_OR_GREATER
				_logger.LogDebug("Enqueueing continuation to run next {Count} jobs in the sequence '{SequnceRecurringJobId}'.", nextRecurringJobIdsToRunInSequence.Length, sequenceRecurringJobId);
#endif
				// parameters are serialized!
				_backgroundJobClient.ContinueJobWith(jobId, () => EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions), jobContinuationOptions);
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