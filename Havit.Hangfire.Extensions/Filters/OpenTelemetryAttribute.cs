using System.Diagnostics;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Havit.Hangfire.Extensions.RecurringJobs.Services;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Logs Hangfire jobs using OpenTelemetry tracing.
/// </summary>
public class OpenTelemetryAttribute : JobFilterAttribute, IServerFilter
{
	private static readonly ActivitySource ActivitySource = new ActivitySource("Havit.Hangfire.Extensions");

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

		Activity activity = ActivitySource.StartActivity(activityName, ActivityKind.Server); // ActivityKind.Server: Mapuje se do ApplicationInsights jako Request
		activity?.SetTag("hangfire.job.id", context.BackgroundJob.Id);

		context.Items["OpenTelemetryActivity"] = activity;
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext context)
	{
		Activity activity = null;

		if (context.Items.TryGetValue("OpenTelemetryActivity", out object activityObject))
		{
			activity = (Activity)activityObject;
		}

		if (activity == null)
		{
			return;
		}

		if ((context.Exception == null) || context.ExceptionHandled)
		{
			activity.SetStatus(ActivityStatusCode.Ok);
		}
		else
		{
			activity.AddException(context.Exception);
			activity.SetStatus(ActivityStatusCode.Error);
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
