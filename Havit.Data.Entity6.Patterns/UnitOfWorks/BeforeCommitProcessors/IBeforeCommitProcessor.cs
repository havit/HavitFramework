namespace Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Procesor, který se spustí před provedením Commitu na UoW.
/// </summary>
public interface IBeforeCommitProcessor<in TEntity>
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	void Run(ChangeType changeType, TEntity changingEntity);
}
