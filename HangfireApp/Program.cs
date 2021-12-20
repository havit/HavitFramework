﻿using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Console;
using Hangfire.SqlServer;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;
using Havit.Hangfire.Extensions.BackgroundJobs;
using Havit.Hangfire.Extensions.Filters;
using Havit.Hangfire.Extensions.RecurringJobs;
using Havit.HangfireApp.Jobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireApp
{
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
					services.AddHttpClient<IJobOne, JobOne>(c => c.BaseAddress = new Uri("https://www.havit.cz"));
					services.AddTransient<IJobTwo, JobTwo>();
					services.AddTransient<IJobThree, JobThree>();

					services.AddApplicationInsightsTelemetryWorkerService();
					services.AddExceptionMonitoring(hostContext.Configuration);

					if (useHangfire)
					{
						services.AddHangfire((serviceProvider, configuration) => configuration
							.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
							.UseSimpleAssemblyNameTypeSerializer()
							.UseRecommendedSerializerSettings()
							.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(hostContext.Configuration.GetConnectionString("Database")), new SqlServerStorageOptions
							{
								CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
								SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
								QueuePollInterval = TimeSpan.Zero,
								UseRecommendedIsolationLevel = true,
								DisableGlobalLocks = true,
							})
							.WithJobExpirationTimeout(TimeSpan.FromDays(30)) // historie hangfire
							// TODO: Application Insights...
							.UseFilter(new AutomaticRetryAttribute { Attempts = 0 }) // do not retry failed jobs
							.UseFilter(new CancelRecurringJobWhenAlreadyInQueueOrCurrentlyRunningFilter()) // joby se (v případě "nestihnutí" zpracování) nezařazují opakovaně
							.UseFilter(new ExceptionMonitoringAttribute(serviceProvider.GetRequiredService<IExceptionMonitoringService>())) // zajistíme hlášení chyby v případě selhání jobu
							.UseConsole()
						);

#if DEBUG
						services.AddHangfireEnqueuedJobsCleanupOnApplicationStartup();
#endif
						services.AddHangfireRecurringJobsSchedulerOnApplicationStartup(GetRecurringJobsToSchedule().ToArray());

						// Add the processing server as IHostedService
						services.AddHangfireServer(o => o.WorkerCount = 1);
					}
				});

			// V případě registrace více lifetimes je použit poslední zaregistrovaný.
			if (useHangfire)
			{
				hostBuidler.UseWebJobLifetime(); // aplikaci lze ukončit pomocí WebJob graceful shutdownu
			}
			else
			{
				hostBuidler.UseConsoleLifetime(); // aplikaci lze ukončit pomocí Ctrl+C
			}

			// TODO: Zkusit CommandLineHostService?

			IHost host = hostBuidler.Build();

			if (useHangfire)
			{
				await host.RunAsync();
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

		private static void ShowCommandsHelp()
		{
			Console.WriteLine("Supported commands:");
			foreach (var job in GetRecurringJobsToSchedule().OrderBy(job => job.JobId).ToList())
            {
				Console.WriteLine("  " + job.JobId);
			}
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

			try
			{
				using (var scopeService = serviceProvider.CreateScope())
				{
					await job.RunAsync(scopeService.ServiceProvider);
				}
			}
			catch (Exception ex)
			{
				var service = serviceProvider.GetRequiredService<IExceptionMonitoringService>();
				service.HandleException(ex);

				throw;
			}

			return true;
		}

		private static IEnumerable<IRecurringJob> GetRecurringJobsToSchedule()
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

			yield return new RecurringJob<IJobOne>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
			yield return new RecurringJob<IJobTwo>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
			yield return new RecurringJob<IJobThree>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
		}
	}

	// TODO: Nová knihovna v HFW?
	// TODO: Nejprvne vyzkoušet
		public class WebJobLifetime : IHostLifetime, IDisposable
		{
			private readonly IHostApplicationLifetime applicationLifetime;
			private readonly ILogger logger;

			private WebJobsShutdownWatcher webJobsShutdownWatcher;
			private CancellationTokenRegistration applicationStartedRegistration;

			public WebJobLifetime(IHostApplicationLifetime applicationLifetime, ILogger<WebJobLifetime> logger)
			{
				this.applicationLifetime = applicationLifetime;
				this.logger = logger;
			}

			public Task WaitForStartAsync(CancellationToken cancellationToken)
			{
				webJobsShutdownWatcher = new WebJobsShutdownWatcher();
				if (webJobsShutdownWatcher.Token != CancellationToken.None)
				{
					// Pattern from ConsoleLifetime
					applicationStartedRegistration = applicationLifetime.ApplicationStarted.Register(state =>
					{
						((WebJobLifetime)state).OnApplicationStarted();
					}, this);

					webJobsShutdownWatcher.Token.Register(() =>
					{
						applicationLifetime.StopApplication();
					});
				}

				//  Applications start immediately.
				return Task.CompletedTask;
			}

			private void OnApplicationStarted()
			{
				logger.LogInformation("Waiting for WebJob shutdown signal.");
			}

			public Task StopAsync(CancellationToken cancellationToken)
			{
				// There's nothing to do here
				return Task.CompletedTask;
			}

			public void Dispose()
			{
				applicationStartedRegistration.Dispose();
				webJobsShutdownWatcher?.Dispose();
			}
		}

		public static class Extensions
		{
			public static IHostBuilder UseWebJobLifetime(this IHostBuilder hostBuilder)
			{
				return hostBuilder.ConfigureServices(services => services.AddSingleton<IHostLifetime, WebJobLifetime>());
			}
		}
	}