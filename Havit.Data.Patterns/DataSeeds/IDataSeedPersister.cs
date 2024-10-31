namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Zajištuje persistenci (uložení) seedovaných dat (vč. zjištění, které záznamy existují, tj. které založit, které aktualizovat, atp.)
/// </summary>
public interface IDataSeedPersister
{
	/// <summary>
	/// Ukládá seedovaná data.
	/// </summary>
	void Save<TEntity>(DataSeedConfiguration<TEntity> dataSeed)
		where TEntity : class;

	/// <summary>
	/// Ukládá seedovaná data.
	/// </summary>
	Task SaveAsync<TEntity>(DataSeedConfiguration<TEntity> configuration, CancellationToken cancellationToken)
		where TEntity : class;
}
