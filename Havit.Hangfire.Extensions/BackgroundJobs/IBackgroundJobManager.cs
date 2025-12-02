using Hangfire.States;

namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
public interface IBackgroundJobManager
{
	/// <summary>
	/// Deletes all enqueued jobs in a queue.
	/// </summary>
	void DeleteEnqueuedJobs(string queue = EnqueuedState.DefaultQueue);
}