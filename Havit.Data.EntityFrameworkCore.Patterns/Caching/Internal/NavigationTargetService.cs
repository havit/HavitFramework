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
					.SelectMany(entityType => entityType
						.GetNavigations()
						.Select(navigation => new
						{
							DeclaringClrType = entityType.ClrType,
							TargetClrType = navigation.TargetEntityType.ClrType,
							PropertyName = navigation.Name,
							NavigationType = GetNavigationType(navigation)
						})
						.Concat(entityType.GetSkipNavigations()
								.Where(skipNavigation => skipNavigation.PropertyInfo != null)
								.Select(skipNavigation => new
								{
									DeclaringClrType = entityType.ClrType,
									TargetClrType = skipNavigation.TargetEntityType.ClrType,
									PropertyName = skipNavigation.Name,
									NavigationType = GetNavigationType(skipNavigation)
								})))
					.ToDictionary(
						a => new TypePropertyName(a.DeclaringClrType, a.PropertyName),
						a => new NavigationTarget
						{
							TargetClrType = a.TargetClrType,
							NavigationType = a.NavigationType
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

	private NavigationType GetNavigationType(IReadOnlyNavigation navigation)
	{
		if (navigation.ForeignKey.DeclaringEntityType == navigation.DeclaringType)
		{
			return NavigationType.Reference;
		}

		return navigation.IsCollection
			? navigation.TargetEntityType.IsManyToManyEntity() ? NavigationType.ManyToManyDecomposedToOneToMany : NavigationType.OneToMany
			: NavigationType.OneToOne;
	}

	private NavigationType GetNavigationType(IReadOnlySkipNavigation navigation)
	{
		return NavigationType.ManyToMany;
	}
}
