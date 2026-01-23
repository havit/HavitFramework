using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.SqlServer;
using Hangfire.States;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;
using Havit.ApplicationInsights.DependencyCollector;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;
using Havit.Hangfire.Extensions.BackgroundJobs;
using Havit.Hangfire.Extensions.Filters;
using Havit.Hangfire.Extensions.RecurringJobs;
using Havit.Hangfire.Extensions.Tags;
using Havit.HangfireJobs.Jobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Havit.HangfireApp;

public static class Program
{
	public static async Task Main(string[] args)
	{
		bool useHangfire = args.Length == 0;

		IHostBuilder hostBuidler = Host.CreateDefaultBuilder();
		hostBuidler.ConfigureAppConfiguration((hostContext, config) =>
			{
				config
					.AddJsonFile(@"appsettings.HangfireApp.json", optional: false)
					.AddJsonFile($"appsettings.HangfireApp.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables();
			})
			.ConfigureServices((hostContext, services) =>
			{
				services.AddTransient<IJobOne, JobOne>();
				services.AddTransient<IJobTwo, JobTwo>();
				services.AddTransient<IJobThree, JobThree>();

				services.AddApplicationInsightsTelemetryWorkerService();
				services.AddApplicationInsightsTelemetryProcessor<IgnoreSucceededDependenciesWithNoParentIdProcessor>(); // ignorujeme infrastrukturní položky Hangfire (předpokládá použití ApplicationInsightAttribute níže)
				services.Remove(services.Single(descriptor => descriptor.ImplementationType == typeof(PerformanceCollectorModule))); // odebereme hlášení PerformanceCounters

				services.AddExceptionMonitoring(hostContext.Configuration);

				if (useHangfire)
				{
					services.AddHangfire((serviceProvider, configuration) => configuration
						.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
						.UseSimpleAssemblyNameTypeSerializer()
						.UseRecommendedSerializerSettings()
						.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(hostContext.Configuration.GetConnectionString("Database")), new SqlServerStorageOptions
						{
							CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
							SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
							QueuePollInterval = TimeSpan.FromSeconds(5),
							UseRecommendedIsolationLevel = true,
							DisableGlobalLocks = true,
							EnableHeavyMigrations = true
						})
						.WithJobExpirationTimeout(TimeSpan.FromDays(30)) // historie hangfire
						.UseTagsWithSql(new TagsOptions { Clean = Clean.None })
						.UseJobsTagging()
						.UseTagsDashboardExtension()
						.UseFilter(new AutomaticRetryAttribute { Attempts = 0, LogEvents = false }) // do not retry failed jobs						
						.UseFilter(new FinalFailedStateFilter()) // zapojíme FinalFailedState, který zajistí expiraci i failovaných jobů
						.UseFilter(new ContinuationsSupportAttribute(new HashSet<string> { FailedState.StateName, DeletedState.StateName, SucceededState.StateName })) // only working with AutomaticRetryAttribute with Attempts = 0
						.UseFilter(new CancelRecurringJobWhenAlreadyInQueueOrCurrentlyRunningFilter()) // joby se (v případě "nestihnutí" zpracování) nezařazují opakovaně
						.UseFilter(new DeleteSequenceRecurringJobSchedulerFilter()) // zajistí odstranění systémových stavů jobů zajišťujících běh recurring jobů v sekvenci
						.UseFilter(new ExceptionMonitoringAttribute(serviceProvider.GetRequiredService<IExceptionMonitoringService>())) // zajistíme hlášení chyby v případě selhání jobu
						.UseFilter(new ApplicationInsightAttribute(serviceProvider.GetRequiredService<TelemetryClient>()) { JobNameFunc = backgroundJob => Hangfire.Extensions.Helpers.JobNameHelper.TryGetSimpleName(backgroundJob.Job, out string simpleName) ? simpleName : backgroundJob.Job.ToString() })
						.UseConsole()
					);

					services.AddHangfireConsoleExtensions(); // adds support for Hangfire jobs logging  to a dashboard using ILogger<T> (.UseConsole() in hangfire configuration is required!)
					services.AddHangfireSequenceRecurringJobScheduler();

#if DEBUG
					services.AddHangfireEnqueuedJobsCleanupOnApplicationStartup();
#endif
					services.AddHangfireRecurringJobsSchedulerOnApplicationStartup(GetRecurringJobsToSchedule().ToArray());

					// Add the processing server as IHostedService
					services.AddHangfireServer(o => o.WorkerCount = 1);
				}
			});

		IHost host = hostBuidler.Build();

		if (useHangfire)
		{
			// Run with Hangfire
			using (WebJobsShutdownWatcher webJobsShutdownWatcher = new WebJobsShutdownWatcher())
			{
				await host.RunAsync(webJobsShutdownWatcher.Token);
			}
		}
		else
		{
			// Run with command line
			if ((args.Length > 1) || (!await TryRunCommandAsync(host.Services, args[0])))
			{
				ShowCommandsHelp();
			}
		}
	}

	private static IEnumerable<IRecurringJob> GetRecurringJobsToSchedule()
	{
		RecurringJobOptions recurringJobOptions = new RecurringJobOptions
		{
			TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"),
		};

		var job1 = new RecurringJob<IJobOne>(EnqueuedState.DefaultQueue, job => job.ExecuteAsync(CancellationToken.None), Cron.Never(), recurringJobOptions);
		var job2 = new RecurringJob<IJobTwo>(EnqueuedState.DefaultQueue, job => job.ExecuteAsync(CancellationToken.None), Cron.Never(), recurringJobOptions);
		var job3 = new RecurringJob<IJobThree>(EnqueuedState.DefaultQueue, job => job.ExecuteAsync(CancellationToken.None), Cron.Never(), recurringJobOptions);

		yield return job1;
		yield return job2;
		yield return job3;

		yield return new SequenceRecurringJob("All three jobs", EnqueuedState.DefaultQueue, Cron.Never(), recurringJobOptions, new IRecurringJob[] { job1, job2, job3 });
	}

	private static async Task<bool> TryRunCommandAsync(IServiceProvider serviceProvider, string command)
	{
		Contract.Requires<ArgumentNullException>(serviceProvider != null, nameof(serviceProvider));
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(command));

		var job = GetRecurringJobsToSchedule().SingleOrDefault(job => String.Equals(job.JobId, command, StringComparison.CurrentCultureIgnoreCase));
		if (job == null)
		{
			return false;
		}

		using (var scopeService = serviceProvider.CreateScope())
		{
			IExceptionMonitoringService exceptionMonitoringService = serviceProvider.GetRequiredService<IExceptionMonitoringService>();
			try
			{
				await job.RunAsync(scopeService.ServiceProvider, CancellationToken.None);
			}
			catch (Exception ex)
			{
				exceptionMonitoringService.HandleException(ex);

				throw;
			}
		}

		return true;
	}

	private static void ShowCommandsHelp()
	{
		Console.WriteLine("Supported commands:");
		foreach (var job in GetRecurringJobsToSchedule().OrderBy(job => job.JobId).ToList())
		{
			Console.WriteLine("  " + job.JobId);
		}
	}
}