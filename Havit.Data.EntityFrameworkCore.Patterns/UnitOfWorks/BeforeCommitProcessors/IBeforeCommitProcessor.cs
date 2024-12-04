using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Procesor, který se spustí před provedením Commitu na UoW.
/// </summary>
public interface IBeforeCommitProcessor<in TEntity> : IBeforeCommitProcessorInternal
{
	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	ChangeTrackerImpact Run(ChangeType changeType, TEntity changingEntity);

	/// <summary>
	/// Template metoda pro provedení akce před Commitem na UoW.
	/// </summary>
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
