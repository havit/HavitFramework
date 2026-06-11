
namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Bázová třída pro procesor, který se spustí před provedením Commitu na UoW.
/// </summary>
/// <remarks>
/// Implementace mají přepsat právě jednu z metod <see cref="Run(ChangeType, TEntity)" /> nebo <see cref="RunAsync(ChangeType, TEntity, CancellationToken)" />.
/// Runner volá obě metody nad stejnou instancí procesoru; neponechaná výchozí implementace by proto spustila aplikační logiku dvakrát.
/// </remarks>
public abstract class BeforeCommitProcessor<TEntity> : IBeforeCommitProcessor<TEntity>
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <remarks>
	/// Přepište tuto metodu jen pro synchronní procesor. Asynchronní variantu ponechte ve výchozí implementaci vracející <see cref="ChangeTrackerImpact.NoImpact" />.
	/// </remarks>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	public virtual ChangeTrackerImpact Run(ChangeType changeType, TEntity changingEntity)
	{
		return ChangeTrackerImpact.NoImpact;
	}

	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <remarks>
	/// Přepište tuto metodu jen pro asynchronní procesor. Synchronní variantu ponechte ve výchozí implementaci vracející <see cref="ChangeTrackerImpact.NoImpact" />.
	/// </remarks>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public virtual ValueTask<ChangeTrackerImpact> RunAsync(ChangeType changeType, TEntity changingEntity, CancellationToken cancellationToken = default)
	{
		return ValueTask.FromResult(ChangeTrackerImpact.NoImpact);
	}
}
