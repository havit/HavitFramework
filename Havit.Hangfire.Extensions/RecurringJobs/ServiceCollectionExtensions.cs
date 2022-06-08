using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Havit.Hangfire.Extensions.RecurringJobs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Deletes all enqueued jobs in a queue.
		/// </summary>
		public static void AddHangfireRecurringJobsSchedulerOnApplicationStartup(this IServiceCollection services, IRecurringJob[] recurringJobs)
		{
			services.TryAddSingleton<IRecurringJobsScheduler, RecurringJobsScheduler>();
			services.AddHostedService<RecurringJobsSchedulerOnApplicationStartup>();
			services.PostConfigure<RecurringJobsSchedulerOnApplicationStartupOptions>(options => options.RecurringJobs.AddRange(recurringJobs));
		}
	}
}
