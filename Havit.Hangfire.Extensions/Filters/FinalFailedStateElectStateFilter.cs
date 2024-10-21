using Hangfire.States;
using Havit.Hangfire.Extensions.States;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Zajistí náhradu stavu <see cref="FailedState" /> za <see cref="FinalFailedState"/>.
/// Cílem je, aby FailedState měl vlastnost IsFinal nastavenu na true, čímž dojde k expiraci položek v databázi.
/// </summary>
public class FinalFailedStateFilter : IElectStateFilter
{
	/// <inheritdoc />
	public void OnStateElection(ElectStateContext context)
	{
		// Pokud je cílový stav FailedState, nahradíme ho FinalFailedState.
		if (context.CandidateState.GetType() == typeof(FailedState)) // nemůžeme použít is/as!!!
		{
			context.CandidateState = FinalFailedState.From((FailedState)context.CandidateState);
		}
	}
}