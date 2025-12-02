using Hangfire;
using Hangfire.States;

namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
public class BackgroundJobHelper
{
	/// <summary>
	/// Deletes all enqueued jobs in a queue.
	/// </summary>
	public static void DeleteEnqueuedJobs(string queue = EnqueuedState.DefaultQueue)
	{
		var backgroundJobManager = new BackgroundJobManager(new BackgroundJobClient(), JobStorage.Current);
		backgroundJobManager.DeleteEnqueuedJobs(queue);
	}
}