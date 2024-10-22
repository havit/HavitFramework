using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetService : INavigationTargetService
{
	private readonly INavigationTargetStorage _navigationTargetStorage;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public NavigationTargetService(INavigationTargetStorage navigationTargetStorage)
	{
		_navigationTargetStorage = navigationTargetStorage;
	}

	/// <inheritdoc />
	public NavigationTarget GetNavigationTarget(Type type, string propertyName)
	{
		var typePropertyName = new TypePropertyName { Type = type, PropertyName = propertyName }; // TypePropertyName: struktura --> nevytváří alokace na heapu
		if (_navigationTargetStorage.Value.TryGetValue(typePropertyName, out NavigationTarget result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Target type of entity type {0} and property {1} not found.", type.FullName, propertyName));
		}
	}
}
