using Havit.Hangfire.Extensions.RecurringJobs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Hangfire.Extensions.RecurringJobs;

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

	/// <summary>
	/// Adds support for <see cref="SequenceRecurringJob" />.
	/// </summary>
	public static void AddHangfireSequenceRecurringJobScheduler(this IServiceCollection services)
	{
		services.TryAddSingleton<SequenceRecurringJobScheduler>();
	}
}
