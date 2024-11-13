namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Výsledek zpracování BeforeCommitProcessoru.
/// </summary>
public enum ChangeTrackerImpact
{
	/// <summary>
	/// Změny, které udělal BeforeCommitProcessor, nemají žádný dopad na stav entit v ChangeTrackeru (nezměnil se stav žádné entity, nevznikla nová entita, který by měla být trackována, atp.)
	/// </summary>
	NoImpact = 1,

	/// <summary>
	/// Změny, které udělal BeforeCommitProcessor, mají dopad na stav entit v ChangeTrackeru (změnil se stav některé entity, vznikla nová entita, která by měla být trackována, atp.)
	/// </summary>
	StateChanged = 2
}
