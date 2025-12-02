#if NET8_0_OR_GREATER
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Hangfire.Extensions.BackgroundJobs;

/// <summary>
/// Methods to help with background jobs.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Deletes all enqueued jobs in a queue.
	/// </summary>
	public static void AddHangfireEnqueuedJobsCleanupOnApplicationStartup(this IServiceCollection services, string queue = EnqueuedState.DefaultQueue)
	{
		AddHangfireEnqueuedJobsCleanupOnApplicationStartup(services, new string[] { queue });
	}

	/// <summary>
	/// Deletes all enqueued jobs in a queues.
	/// </summary>
	public static void AddHangfireEnqueuedJobsCleanupOnApplicationStartup(this IServiceCollection services, string[] queues)
	{
		services.TryAddSingleton<IBackgroundJobManager, BackgroundJobManager>();
		services.AddHostedService<EnqueuedJobsCleanupOnApplicationStartup>();
		services.PostConfigure<EnqueuedJobsCleanupOnApplicationStartupOptions>(options => options.Queues.AddRange(queues));
	}
}
#endif