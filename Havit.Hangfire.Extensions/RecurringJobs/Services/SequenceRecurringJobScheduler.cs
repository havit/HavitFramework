using Hangfire;
using Havit.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Ensures running recurrying jobs sequence. Used for <see cref="SequenceRecurringJob" />
/// </summary>
public class SequenceRecurringJobScheduler
{
	private readonly RecurringJobManager _recurringJobManager; // IRecurringJobManager nemá TriggerExection, jenž vrací jobId
	private readonly ILogger<SequenceRecurringJobScheduler> _logger;
	private readonly IBackgroundJobClient _backgroundJobClient;

	/// <summary>
	/// Constructor.
	/// </summary>
	public SequenceRecurringJobScheduler(ILogger<SequenceRecurringJobScheduler> logger, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
	{
		Contract.Requires(recurringJobManager is RecurringJobManager);

		this._logger = logger;
		this._recurringJobManager = (RecurringJobManager)recurringJobManager;
		this._backgroundJobClient = backgroundJobClient;
	}

	/// <summary>
	/// Ensures running recurrying jobs sequence. 
	/// </summary>
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
			_logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a first job in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, sequenceRecurringJobId);
		}
		else
		{
			_logger.LogDebug("Triggering recurring job '{RecurringJobId}' as a continuation of job '{PreviousRecurringJobId}' in the sequence '{SequnceRecurringJobId}'.", recurringJobIdToEnqueue, previousSequenceRecurringJobId, sequenceRecurringJobId);
		}

		string jobId = _recurringJobManager.TriggerJob(recurringJobIdToEnqueue);
		if (jobId == null)
		{
			_logger.LogWarning("Triggering recurring job '{RecurringJobId}' failed.", recurringJobIdToEnqueue);
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
				_logger.LogDebug("Continuing with next jobs...");
				// pokud naplánování spuštění dané naplánované úlohy selhalo, a můžeme pokračovat i v případě neúspěchu,
				// pokračujeme v naplánování dalšího kroku
				EnqueueNextRecurringJob(sequenceRecurringJobId, recurringJobIdToEnqueue, nextRecurringJobIdsToRunInSequence, jobContinuationOptions);
			}
			else
			{
				_logger.LogDebug("Enqueueing continuation to run next {Count} jobs in the sequence '{SequnceRecurringJobId}'.", nextRecurringJobIdsToRunInSequence.Length, sequenceRecurringJobId);

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