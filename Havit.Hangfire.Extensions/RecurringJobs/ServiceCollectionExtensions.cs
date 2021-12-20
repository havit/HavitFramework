using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
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
			services.TryAddSingleton<IRecurringJobsHelperService, RecurringJobsHelperService>();
			services.AddSingleton<IHostedService>(sp => new RecurringJobsSchedulerOnApplicationStartup(sp.GetRequiredService<IRecurringJobsHelperService>(), recurringJobs));
		}
	}
}
