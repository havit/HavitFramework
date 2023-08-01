namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// </summary>
public interface IEntityLookupDataStorage
{
	/// <summary>
	/// Vrátí lookup data pro konzumenta.
	/// Pokud nejsou data k dispozici, jsou založena prostřednictvím factory (pokud je factory null, nejsou založena a metoda vrací null).
	/// </summary>
	/// <param name="storageKey">Klíč, pod jakým jsou lookup data v evidenci.</param>
	/// <param name="factory">Pokud nejsou lookup data v evidenci, factory je založí.</param>
	EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey, Func<EntityLookupData<TEntity, TEntityKey, TLookupKey>> factory);

	/// <summary>
	/// Odstraní data evidované pro konzumenta.
	/// </summary>
	void RemoveEntityLookupData(string storageKey);
}
