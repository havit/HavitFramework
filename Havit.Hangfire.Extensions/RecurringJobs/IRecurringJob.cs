namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Recurrying job to schedule.
	/// </summary>
	public interface IRecurringJob
	{
		/// <summary>
		/// Job identifier.
		/// </summary>
		string JobId { get; }

		/// <summary>
		/// Schedules the job.
		/// </summary>
		void Schedule();
	}
}