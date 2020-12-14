using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hangfire;

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Recurring job to schedule.
	/// </summary>
	public class RecurringJob<TJob> : IRecurringJob
	{
		/// <summary>
		/// Job identifier. Takes TJob class name (for interfaces it trims starting I).
		/// </summary>
		public string JobId
		{
			get
			{
				return (typeof(TJob).IsInterface && typeof(TJob).Name.StartsWith("I"))
					? typeof(TJob).Name.Substring(1)
					: typeof(TJob).Name;
			}
		}

		/// <summary>
		/// Returns the jobs.
		/// </summary>
		public Expression<Func<TJob, Task>> MethodCall { get; }

		/// <summary>
		/// Cron expression.
		/// </summary>
		public string CronExpression { get; }

		/// <summary>
		/// Time zone info.
		/// </summary>
		public TimeZoneInfo TimeZone { get; }

		/// <summary>
		/// Queue name.
		/// </summary>
		public string Queue { get;  }

		/// <summary>
		/// Constructor.
		/// </summary>
		public RecurringJob(Expression<Func<TJob, Task>> methodCall, string cronExpression, TimeZoneInfo timeZone, string queue = "default")
		{
			MethodCall = methodCall;
			CronExpression = cronExpression ?? Cron.Never();
			TimeZone = timeZone;
			Queue = queue;
		}

		/// <inheritdoc />
		public void Schedule()
		{
			RecurringJob.AddOrUpdate<TJob>(JobId, MethodCall, CronExpression, TimeZone);
		}
	}
}
