using System;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Client;
using Hangfire.States;

namespace Havit.Hangfire.Extensions.Filters
{
	/// <summary>
	/// Cancels the background job when enqueueing the job from recurring job and and it is currently enqueued or running (processing).
	/// Handles the situation when job is marked running on a dead server.
	/// The implementation is not bullet-proof but good-enought.
	/// </summary>
	public class CancelRecurringJobWhenAlreadyInQueueOrCurrentlyRunningFilter : JobFilterAttribute, IClientFilter, IApplyStateFilter
	{
		private const string EnqueuedHashName = "Enqueued";
		private const string RunningServerIdHashName = "RunningServerId";

		/// <inheritdoc />
		public void OnCreating(CreatingContext context)
		{
			if (!(context.Connection is JobStorageConnection connection))
			{
				return;
			}

			if (!context.Parameters.ContainsKey("RecurringJobId"))
			{
				return;
			}

			var recurringJobId = context.Parameters["RecurringJobId"] as string;
			if (string.IsNullOrWhiteSpace(recurringJobId))
			{
				return;
			}

			if (bool.TryParse(connection.GetValueFromHash($"recurring-job:{recurringJobId}", EnqueuedHashName), out bool enqueued) && enqueued)
			{
				// cancelling - already enqueued
				context.Canceled = true;
				return;
			}

			string runningServerId = connection.GetValueFromHash($"recurring-job:{recurringJobId}", RunningServerIdHashName);
			if (!String.IsNullOrEmpty(runningServerId))
			{
				if (JobStorage.Current.GetMonitoringApi().Servers().Any(server => server.Name == runningServerId // job is running o a server
					&& (server.Heartbeat != null) // which heart is beating or was beating
					&& (server.Heartbeat.Value > DateTime.UtcNow.AddMinutes(-5)))) // do not block queing when "running on a dead servers" (constant -5 was copied from Hangfire sources (see "possiblyAbortedThreshold"))
				{
					// cancelling - already running on a live server
					context.Canceled = true;
					return;
				} // else - running on a dead server, keep going
			}

			// otherwise keep going...

		}

		/// <inheritdoc />
		public void OnCreated(CreatedContext filterContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			var recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
			if (String.IsNullOrWhiteSpace(recurringJobId))
			{
				return;
			}

			transaction.SetRangeInHash($"recurring-job:{recurringJobId}", new[]
			{
				new KeyValuePair<string, string>(EnqueuedHashName, (context.NewState is EnqueuedState).ToString()), // currently enqueued?
				new KeyValuePair<string, string>(RunningServerIdHashName, (context.NewState is ProcessingState processingState) ? processingState.ServerId : "")
			});
		}

		/// <inheritdoc />
		public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			// NOOP
		}

	}
}
