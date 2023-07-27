using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetTypeService : INavigationTargetTypeService
{
	private readonly INavigationTargetTypeStorage navigationTargetTypeStorage;
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public NavigationTargetTypeService(INavigationTargetTypeStorage navigationTargetTypeStorage, IDbContext dbContext)
	{
		this.navigationTargetTypeStorage = navigationTargetTypeStorage;
		this.dbContext = dbContext;
	}

	/// <inheritdoc />
	public Type GetNavigationTargetType(Type type, string propertyName)
	{
		if (navigationTargetTypeStorage.Value == null)
		{
			lock (navigationTargetTypeStorage)
			{
				if (navigationTargetTypeStorage.Value == null)
				{
					navigationTargetTypeStorage.Value = dbContext.Model.GetApplicationEntityTypes()
					.SelectMany(entityType =>
						entityType.GetNavigations().Cast<INavigationBase>()
							.Concat(entityType.GetSkipNavigations()
								.Where(skipNavigation => skipNavigation.PropertyInfo != null)
								.Cast<INavigationBase>()))
					.ToDictionary(
						navigation => new TypePropertyName(navigation.DeclaringEntityType.ClrType, navigation.Name),
						navigation => navigation.TargetEntityType.ClrType);
				}
			}
		}

		if (navigationTargetTypeStorage.Value.TryGetValue(new TypePropertyName(type, propertyName), out Type result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Target type of entity type {0} and property {1} not found.", type.FullName, propertyName));
		}
	}
}
