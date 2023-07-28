using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetService : INavigationTargetService
{
	private readonly INavigationTargetStorage navigationTargetStorage;
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public NavigationTargetService(INavigationTargetStorage navigationTargetStorage, IDbContext dbContext)
	{
		this.navigationTargetStorage = navigationTargetStorage;
		this.dbContext = dbContext;
	}

	/// <inheritdoc />
	public NavigationTarget GetNavigationTarget(Type type, string propertyName)
	{
		if (navigationTargetStorage.Value == null)
		{
			lock (navigationTargetStorage)
			{
				if (navigationTargetStorage.Value == null)
				{
					navigationTargetStorage.Value = dbContext.Model.GetApplicationEntityTypes()
					.SelectMany(entityType =>
						entityType.GetNavigations().Cast<INavigationBase>()
							.Concat(entityType.GetSkipNavigations()
								.Where(skipNavigation => skipNavigation.PropertyInfo != null)
								.Cast<INavigationBase>()))
					.ToDictionary(
						navigation => new TypePropertyName(navigation.DeclaringEntityType.ClrType, navigation.Name),
						navigation => new NavigationTarget
						{
							Type = navigation.TargetEntityType.ClrType,
							IsCollection = navigation.IsCollection
						});
				}
			}
		}

		if (navigationTargetStorage.Value.TryGetValue(new TypePropertyName(type, propertyName), out NavigationTarget result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Target type of entity type {0} and property {1} not found.", type.FullName, propertyName));
		}
	}
}
