namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;

/// <summary>
/// Interní rozhraní pro BeforeCommitProcessory. Slouží k eliminaci reflexe při spouštění BeforeCommitProcessorů.
/// </summary>
public interface IBeforeCommitProcessorInternal
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	ChangeTrackerImpact Run(ChangeType changeType, object changingEntity);

	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	ValueTask<ChangeTrackerImpact> RunAsync(ChangeType changeType, object changingEntity, CancellationToken cancellationToken = default);
}