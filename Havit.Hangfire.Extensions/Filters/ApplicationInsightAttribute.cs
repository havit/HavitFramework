using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Logs Hangfire jobs as requests to Application Insights.
/// </summary>
public class ApplicationInsightAttribute : JobFilterAttribute, IServerFilter
{
	private readonly TelemetryClient telemetryClient;

	/// <summary>
	/// Gets the custom name of the job.
	/// </summary>
	public Func<BackgroundJob, string> JobNameFunc { get; set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public ApplicationInsightAttribute(TelemetryClient telemetryClient)
	{
		this.telemetryClient = telemetryClient;
	}

	/// <inheritdoc />
	public void OnPerforming(PerformingContext context)
	{
		RequestTelemetry requestTelemetry = new RequestTelemetry()
		{
			Name = "JOB " + GetJobName(context.BackgroundJob)
		};

		// Track Hangfire Job as a Request (operation) in AI
		IOperationHolder<RequestTelemetry> operation = telemetryClient.StartOperation(requestTelemetry);
		requestTelemetry.Properties.Add("JobId", context.BackgroundJob.Id);

		context.Items["ApplicationInsightsOperation"] = operation;
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext context)
	{
		IOperationHolder<RequestTelemetry> operation = context.Items["ApplicationInsightsOperation"] as IOperationHolder<RequestTelemetry>;

		if (operation != null)
		{
			if (((context.Exception == null) || context.ExceptionHandled))
			{
				operation.Telemetry.Success = true;
				operation.Telemetry.ResponseCode = "Success";
			}
			else
			{
				operation.Telemetry.Success = false;
				operation.Telemetry.ResponseCode = "Failed";

				string operationId = operation.Telemetry.Context.Operation.Id;

				var exceptionTelemetry = new ExceptionTelemetry(context.Exception);
				// See https://docs.microsoft.com/en-us/azure/azure-monitor/app/correlation
				exceptionTelemetry.Context.Operation.Id = operationId;
				exceptionTelemetry.Context.Operation.ParentId = operationId;

				telemetryClient.TrackException(exceptionTelemetry);
			}

			telemetryClient.StopOperation(operation);
		}
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
