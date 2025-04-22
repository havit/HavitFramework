using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje seznam kolekcí a one-to-one referencí referencující danou entitu.
/// </summary>
public class ReferencingNavigationsService : IReferencingNavigationsService
{
	private readonly IReferencingNavigationsStorage _referencingNavigationsStorage;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ReferencingNavigationsService(IReferencingNavigationsStorage referencingNavigationsStorage)
	{
		_referencingNavigationsStorage = referencingNavigationsStorage;
	}

	/// <inheritdoc />
	public List<ReferencingNavigation> GetReferencingNavigations(IEntityType entityType)
	{
		if (_referencingNavigationsStorage.Value.TryGetValue(entityType, out var result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("EntityType {0} is not a supported type.", entityType.Name));
		}
	}
}

