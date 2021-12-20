using Hangfire;
using System;
using System.Threading.Tasks;

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	// TODO: Zpětná kompatibilita? K čemu? Vlastní službu na spouštění?
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
		void ScheduleAsRecurringJob(IRecurringJobManager recurringJobManager);

		/// <summary>
		/// Runs the job immediately.
		/// </summary>
		Task RunAsync(IServiceProvider serviceProvider);
	}
}