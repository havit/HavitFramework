namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Zajišťuje distribuovanou invalidaci lookup dat.
/// </summary>
public interface IDistributedLookupDataInvalidationService
{
	/// <summary>
	/// Invaliduje data v úložišti s daným klíčem.
	/// </summary>
	void Invalidate(string storageKey);
}
