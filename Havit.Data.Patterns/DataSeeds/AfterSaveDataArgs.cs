namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Argumenty pro callback po uložení seedovaného objektu.
/// </summary>
public class AfterSaveDataArgs<TEntity>
{
	/// <summary>
	/// Předpis objektu k seedování.
	/// </summary>
	public TEntity SeedEntity { get; private set; }

	/// <summary>
	/// Objekt, který byl na základě předpisu seedování persistován.
	/// </summary>
	public TEntity PersistedEntity { get; private set; }

	/// <summary>
	/// Pokud byl objekt založen, pak je hodnota true. Pokud byl aktualizován, pak má hodnotu false.
	/// </summary>
	public bool WasNew { get; private set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AfterSaveDataArgs(TEntity seedEntity, TEntity persistedEntity, bool wasNew)
	{
		SeedEntity = seedEntity;
		PersistedEntity = persistedEntity;
		WasNew = wasNew;
	}
}