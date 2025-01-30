using Hangfire.States;
using Havit.Hangfire.Extensions.RecurringJobs.Services;

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