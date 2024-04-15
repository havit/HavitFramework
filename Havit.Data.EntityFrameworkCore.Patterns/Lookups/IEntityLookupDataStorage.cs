namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// </summary>
public interface IEntityLookupDataStorage
{
	/// <summary>
	/// Vrátí lookup data pro konzumenta.
	/// Pokud nejsou data k dispozici, vrací null.
	/// </summary>
	/// <param name="storageKey">Klíč, pod jakým jsou lookup data v evidenci.</param>
	EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey);

	/// <summary>
	/// Uloží lookup data pro konzumenta.
	/// </summary>
	/// <param name="storageKey">Klíč, pod jakým jsou lookup data v evidenci.</param>
	/// <param name="entityLookupData">Storage k uložení.</param>
	void StoreEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey, EntityLookupData<TEntity, TEntityKey, TLookupKey> entityLookupData);

	/// <summary>
	/// Odstraní data evidované pro konzumenta.
	/// </summary>
	void RemoveEntityLookupData(string storageKey);
}
