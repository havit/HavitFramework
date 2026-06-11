using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Procesor, který se spustí před provedením Commitu na UoW.
/// </summary>
/// <remarks>
/// Implementace mají realizovat právě jednu z metod <see cref="Run(ChangeType, TEntity)" /> nebo <see cref="RunAsync(ChangeType, TEntity, CancellationToken)" />.
/// Runner volá obě metody nad stejnou instancí procesoru; druhá metoda má zůstat bez aplikační logiky a vracet <see cref="ChangeTrackerImpact.NoImpact" />.
/// </remarks>
public interface IBeforeCommitProcessor<in TEntity> : IBeforeCommitProcessorInternal
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <remarks>
	/// Synchronní vstupní bod. Nepoužívejte současně s <see cref="RunAsync(ChangeType, TEntity, CancellationToken)" /> pro tutéž aplikační logiku.
	/// </remarks>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	ChangeTrackerImpact Run(ChangeType changeType, TEntity changingEntity);

	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <remarks>
	/// Asynchronní vstupní bod. Nepoužívejte současně s <see cref="Run(ChangeType, TEntity)" /> pro tutéž aplikační logiku.
	/// </remarks>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	ValueTask<ChangeTrackerImpact> RunAsync(ChangeType changeType, TEntity changingEntity, CancellationToken cancellationToken = default);

	#region IBeforeCommitProcessorInternal explicit interface implementation
	ChangeTrackerImpact IBeforeCommitProcessorInternal.Run(ChangeType changeType, object changingEntity)
	{
		// default interface implementation
		return this.Run(changeType, (TEntity)changingEntity);
	}

	ValueTask<ChangeTrackerImpact> IBeforeCommitProcessorInternal.RunAsync(ChangeType changeType, object changingEntity, CancellationToken cancellationToken)
	{
		// default interface implementation
		return this.RunAsync(changeType, (TEntity)changingEntity, cancellationToken);
	}
	#endregion
}
