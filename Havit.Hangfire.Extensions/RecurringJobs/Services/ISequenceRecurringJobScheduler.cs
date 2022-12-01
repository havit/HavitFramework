using Hangfire;

namespace Havit.Hangfire.Extensions.RecurringJobs.Services;

/// <summary>
/// Ensures running recurrying jobs sequence. Used for <see cref="SequenceRecurringJob" />
/// </summary>
public interface ISequenceRecurringJobScheduler
{
	/// <summary>
	/// Ensures running recurrying jobs sequence. 
	/// </summary>
	void ProcessRecurryingJobsInQueue(string sequenceRecurringJobId, string[] recurringJobIdsToRunInSequence, JobContinuationOptions jobContinuationOptions);
}