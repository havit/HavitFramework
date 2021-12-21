using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Options for RecurringJobsSchedulerOnApplicationStartup.
	/// </summary>
	internal class RecurringJobsSchedulerOnApplicationStartupOptions
	{
		/// <summary>
		/// Recurring jobs to schedule.
		/// </summary>
		public List<IRecurringJob> RecurringJobs { get; } = new List<IRecurringJob>();
    }
}
