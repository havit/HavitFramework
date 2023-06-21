namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Argumenty pro callback před uložením seedovaného objektu.
/// </summary>
public class BeforeSaveDataArgs<TEntity>
{
	/// <summary>
	/// Předpis objektu k seedování.
	/// </summary>
	public TEntity SeedEntity { get; private set; }

	/// <summary>
	/// Objekt, který bude na základě předpisu seedování persistován.
	/// </summary>
	public TEntity PersistedEntity { get; private set; }

	/// <summary>
	/// Pokud bude objekt založen, pak je hodnota true. Pokud bude aktualizován, pak má hodnotu false.
	/// </summary>
	public bool IsNew { get; private set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public BeforeSaveDataArgs(TEntity seedEntity, TEntity persistedEntity, bool isNew)
	{
		SeedEntity = seedEntity;
		PersistedEntity = persistedEntity;
		IsNew = isNew;
	}
}