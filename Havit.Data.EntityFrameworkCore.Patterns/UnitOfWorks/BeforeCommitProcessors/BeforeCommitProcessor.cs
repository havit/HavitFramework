
namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Bázová třída pro procesor, který se spustí před provedením Commitu na UoW.
/// </summary>
public abstract class BeforeCommitProcessor<TEntity> : IBeforeCommitProcessor<TEntity>
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	public virtual ChangeTrackerImpact Run(ChangeType changeType, TEntity changingEntity)
	{
		return ChangeTrackerImpact.NoImpact;
	}

	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public virtual ValueTask<ChangeTrackerImpact> RunAsync(ChangeType changeType, TEntity changingEntity, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult(ChangeTrackerImpact.NoImpact);
	}
}
