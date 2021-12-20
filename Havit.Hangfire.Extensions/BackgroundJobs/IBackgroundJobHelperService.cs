namespace Havit.Hangfire.Extensions.BackgroundJobs
{
    // TODO: Naming?

    /// <summary>
    /// Methods to help with background jobs.
    /// </summary>
    public interface IBackgroundJobHelperService
    {
        /// <summary>
		/// Deletes all enqueued jobs in a queue.
        /// </summary>
        void DeleteEnqueuedJobs(string queue = "default");
    }
}