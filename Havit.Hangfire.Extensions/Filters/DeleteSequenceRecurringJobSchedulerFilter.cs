using System;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Client;
using Hangfire.States;
using Hangfire.Storage.Monitoring;
using Havit.Hangfire.Extensions.RecurringJobs.Services;
using System.Net.Mime;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Úspěšně dokončený SequenceRecurringJobScheduler je smazán, ať se nemotá na dashboardu.
/// </summary>
public class DeleteSequenceRecurringJobSchedulerFilter : IElectStateFilter
{
	/// <inheritdoc/>
	public void OnStateElection(ElectStateContext context)
	{
		if ((context.CandidateState.Name == global::Hangfire.States.SucceededState.StateName) && (context.BackgroundJob.Job.Type == typeof(SequenceRecurringJobScheduler)))
		{
			context.CandidateState = new DeletedState();
		}
	}
}