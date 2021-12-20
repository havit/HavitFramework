using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Havit.Hangfire.Extensions.RecurringJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Hangfire.Extensions.BackgroundJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Deletes all enqueued jobs in a queue.
		/// </summary>
		public static void AddHangfireEnqueuedJobsCleanupOnApplicationStartup(this IServiceCollection services, string queue = "default")
		{
			AddHangfireEnqueuedJobsCleanupOnApplicationStartup(services, new string[] { queue });

		}

		/// <summary>
		/// Deletes all enqueued jobs in a queues.
		/// </summary>
		public static void AddHangfireEnqueuedJobsCleanupOnApplicationStartup(this IServiceCollection services, string[] queues)
		{
			services.TryAddSingleton<IBackgroundJobHelperService, BackgroundJobHelperService>();
			services.AddSingleton<IHostedService>(sp => new EnqueuedJobsCleanupOnApplicationStartup(sp.GetRequiredService<IBackgroundJobHelperService>(), queues));
		}
	}
}
