using System.Diagnostics;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Havit.Core;
using Havit.Hangfire.Extensions.RecurringJobs.Services;
using Havit.Hangfire.Extensions.Telemetry;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Logs Hangfire jobs using OpenTelemetry tracing.
/// </summary>
public class OpenTelemetryJobReportingAttribute : JobFilterAttribute, IServerFilter
{
	/// <summary>
	/// Gets the custom name of the job.
	/// </summary>
	public Func<BackgroundJob, string> JobNameFunc { get; set; }

	/// <inheritdoc />
	public void OnPerforming(PerformingContext context)
	{
		if (context.BackgroundJob.Job.Type == typeof(SequenceRecurringJobScheduler))
		{
			return;
		}

		string jobName = GetJobName(context.BackgroundJob);
		string activityName = "JOB " + jobName;

		Activity activity = ActivitySources.HavitHangfireActivitySource.StartActivity(activityName, ActivityKind.Server); // ActivityKind.Server: Mapuje se do ApplicationInsights jako Request
		activity?.SetTag("hangfire.job.id", context.BackgroundJob.Id);

		context.Items["OpenTelemetryActivity"] = activity;
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext context)
	{
		Activity activity = null;

		// Mohli bychom použít Activity.Current, ale pro jistotu použijeme aktivitu uloženou v kontextu, abychom se vyhnuli kolizím, pokud někdo v kódu založí jinou aktivitu
		if (context.Items.TryGetValue("OpenTelemetryActivity", out object activityObject))
		{
			activity = (Activity)activityObject;
		}

		if (activity == null)
		{
			return;
		}

		if (activity.Status == ActivityStatusCode.Unset)
		{
			if ((context.Exception == null) || context.ExceptionHandled)
			{
				activity.SetStatus(ActivityStatusCode.Ok);
			}
			else
			{
				activity.SetStatus(ActivityStatusCode.Error);
				if ((context.Exception is JobPerformanceException) && CancellationExceptionChecker.IsCancellationException(context.Exception.InnerException))
				{
					// začne fungovat po vydání nové verze Azure.Monitor.OpenTelemetry.AspNetCore
					activity.SetTag("microsoft.request.resultCode", 499); // 499 Client Closed Request (neoficiální status code, ale běžně používaný pro označení zrušení klientem)
				}
				else
				{
					activity.AddException(context.Exception); // useful when exception is not reported via logging
				}
			}
		}

		activity.Dispose();
	}

	private string GetJobName(BackgroundJob backgroundJob)
	{
		if (JobNameFunc != null)
		{
			return JobNameFunc.Invoke(backgroundJob);
		}

		return backgroundJob.Job.ToString();
	}
}
