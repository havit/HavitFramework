namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Spouští registrované IBeforeCommitProcessory.	
/// </summary>
public interface IBeforeCommitProcessorsRunner
{
	/// <summary>
	/// Spustí IBeforeCommitProcessory pro zadané změny. Bez podpory pro asynchronní before commit procesory.
	/// </summary>
	ChangeTrackerImpact Run(Changes changes);

	/// <summary>
	/// Spustí IBeforeCommitProcessory pro zadané změny. Bez podpory pro asynchronní before commit procesory.
	/// </summary>
	ValueTask<ChangeTrackerImpact> RunAsync(Changes changes, CancellationToken cancellationToken);
}
