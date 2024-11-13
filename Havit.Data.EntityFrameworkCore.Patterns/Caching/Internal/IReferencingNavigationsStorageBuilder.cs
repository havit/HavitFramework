namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Sestaví (I)ReferencingNavigationsStorage.
/// </summary>
public interface IReferencingNavigationsStorageBuilder
{
	/// <summary>
	/// Sestaví (I)ReferencingNavigationsStorage.
	/// </summary>
	IReferencingNavigationsStorage Build();
}