using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.UverovaPlatforma.Utility.Hangfire
{
	/// <summary>
	/// Logs Hangfire jobs as requests to Application Insights.
	/// </summary>
	public class ApplicationInsightAttribute : JobFilterAttribute, IServerFilter
	{
		private readonly TelemetryClient telemetryClient;

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
			RequestTelemetry requestTelemetry = new RequestTelemetry
			{
				Name = $"JOB {context.BackgroundJob.Job.Type.Name}.{context.BackgroundJob.Job.Method.Name}",
			};

			// Track Hangfire Job as a Request (operation) in AI
			IOperationHolder<RequestTelemetry> operation = telemetryClient.StartOperation(requestTelemetry);

			requestTelemetry.Properties.Add("JobId", context.BackgroundJob.Id);			
			requestTelemetry.Properties.Add("JobCreatedAt", context.BackgroundJob.CreatedAt.ToString("O"));

			context.Items["ApplicationInsightsOperation"] = operation;
		}

		/// <inheritdoc />
		public void OnPerformed(PerformedContext context)
		{
			IOperationHolder<RequestTelemetry> operation = context.Items["ApplicationInsightsOperation"] as IOperationHolder<RequestTelemetry>;

			if (operation != null)
			{
				if ((context.Exception != null) && !context.ExceptionHandled)
				{
					operation.Telemetry.Success = false;
					operation.Telemetry.ResponseCode = "Failed";

					telemetryClient.TrackException(context.Exception.InnerException ?? context.Exception);
				}
				else
				{
					operation.Telemetry.Success = true;
					operation.Telemetry.ResponseCode = "Success";
				}

				telemetryClient.StopOperation(operation);
			}
		}

	}

}
