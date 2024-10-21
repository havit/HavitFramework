namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Spouští registrované IBeforeCommitProcessory.	
/// </summary>
public interface IBeforeCommitProcessorsRunner
{
	/// <summary>
	/// Spustí registrované IBeforeCommitProcessory.	
	/// </summary>
	ChangeTrackerImpact Run(Changes changes);
}
