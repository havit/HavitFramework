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
			if (args.Length > 0)
			{
				string command = args[0];

				Console.WriteLine($"Command: {command}");

				bool successfullyCompleted =
					await TryDoCommand<IJobOne>(command, "JobOne")
					|| await TryDoCommand<IJobTwo>(command, "JobTwo")
					|| await TryDoCommand<IJobThree>(command, "JobThree");

				if (!successfullyCompleted)
				{
					Console.WriteLine("Nepodařilo se zpracovat příkaz: {0}", command);
					Console.WriteLine();

					ShowCommandsHelp();
				}
			}
			else
			{
				await RunHangfireServer();
			}
		}

		private static void ShowCommandsHelp()
		{
			Console.WriteLine("Podporované příkazy jsou:");
			Console.WriteLine("  JobOne");
			Console.WriteLine("  JobTwo");
			Console.WriteLine("  JobThree");
		}

		private static async Task<bool> TryDoCommand<TJob>(string command, string commandPattern)
			where TJob : class, IRunnableJob
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(command));
			Contract.Requires<ArgumentNullException>(commandPattern != null);

			if (!String.Equals(command, commandPattern, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			await ExecuteWithServiceProvider(async serviceProvider =>
			{
				try
				{
					using (var scopeService = serviceProvider.CreateScope())
					{
						TJob job = scopeService.ServiceProvider.GetRequiredService<TJob>();
						await job.ExecuteAsync(CancellationToken.None);
					}
				}
				catch (Exception ex)
				{
					var service = serviceProvider.GetRequiredService<IExceptionMonitoringService>();
					service.HandleException(ex);

					throw;
				}
			});

			return true;
		}

		private static IEnumerable<IRecurringJob> GetRecurringJobsToSchedule()
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

			yield return new RecurringJob<IJobOne>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
			yield return new RecurringJob<IJobTwo>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
			yield return new RecurringJob<IJobThree>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
		}

		private static async Task RunHangfireServer()
		{
			string connectionString = Configuration.Value.GetConnectionString("Database");

			await ExecuteWithServiceProvider(async (serviceProvider) =>
			{
				GlobalConfiguration.Configuration
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseFilter(new AutomaticRetryAttribute { Attempts = 0 }) // do not retry failed jobs
					.UseFilter(new CancelRecurringJobWhenAlreadyInQueueOrCurrentlyRunningFilter())
					.UseActivator(new AspNetCoreJobActivator(serviceProvider.GetRequiredService<IServiceScopeFactory>()))
					.UseInMemoryStorage()
					//.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(connectionString), new SqlServerStorageOptions
					//{
					//	CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					//	SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					//	QueuePollInterval = TimeSpan.FromSeconds(5),
					//	UseRecommendedIsolationLevel = true,
					//	DisableGlobalLocks = true // Migration to Schema 7 is required
					//	})
					.UseLogProvider(new AspNetCoreLogProvider(serviceProvider.GetRequiredService<ILoggerFactory>())) // enables .NET Core logging for hangfire server (not jobs!) https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/
					.UseConsole(); // enables logging jobs progress to hangfire dashboard (only using a PerformContext class; for ILogger<> support add services.AddHangfireConsoleExtensions())
					JobStorage.Current.JobExpirationTimeout = TimeSpan.FromDays(30); // keep history

#if DEBUG
					BackgroundJobHelper.DeleteEnqueuedJobs();
#endif

					// schedule recurring jobs
					RecurringJobsHelper.SetSchedule(GetRecurringJobsToSchedule().ToArray());

				var options = new BackgroundJobServerOptions
				{
					WorkerCount = 4
				};

				using (var server = new BackgroundJobServer(options))
				{
					if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
					{
							// running in Azure
							Console.WriteLine("Hangfire Server started. Waiting for shutdown signal...");
						using (WebJobsShutdownWatcher webJobsShutdownWatcher = new WebJobsShutdownWatcher())
						{
							await server.WaitForShutdownAsync(webJobsShutdownWatcher.Token);
						}
					}
					else
					{
						// running outside of Azure
						Console.WriteLine("Hangfire Server started. Press Enter to exit...");
						RecurringJob.Trigger("JobOne");
						Console.ReadLine();
					}
				}
			});
		}

		private static async Task ExecuteWithServiceProvider(Func<IServiceProvider, Task> action)
		{
			IConfiguration configuration = Configuration.Value;

			// Setup ServiceCollection
			IServiceCollection services = new ServiceCollection();

			services.AddLogging();
			services.AddHttpClient<IJobOne, JobOne>(c => c.BaseAddress = new Uri("https://www.havit.cz"));
			services.AddTransient<IJobTwo, JobTwo>();
			services.AddTransient<IJobThree, JobThree>();
			services.AddApplicationInsightsTelemetryWorkerService(configuration);

			using (ServiceProvider serviceProvider = services.BuildServiceProvider())
			{
				await action(serviceProvider);
			}
		}

		private static Lazy<IConfiguration> Configuration = new Lazy<IConfiguration>(() =>
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile(@"appsettings.HangfireApp.json", optional: false)
				.AddJsonFile($"appsettings.HangfireApp.{environmentName}.json", optional: true)
				.AddEnvironmentVariables();

			return configurationBuilder.Build();
		});
	}
}

