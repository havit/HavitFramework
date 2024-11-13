using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetStorageBuilder : INavigationTargetStorageBuilder
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public NavigationTargetStorageBuilder(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public INavigationTargetStorage Build()
	{
		return new NavigationTargetStorage
		{
			Value = _dbContext.Model.GetApplicationEntityTypes()
				.SelectMany(entityType => entityType
					.GetNavigations()
					.Select(navigation => new
					{
						TypePropertyName = new TypePropertyName
						{
							Type = entityType.ClrType,
							PropertyName = navigation.Name
						},
						NavigationTarget = new NavigationTarget
						{
							TargetClrType = navigation.TargetEntityType.ClrType,
							NavigationType = GetNavigationType(navigation),
							PropertyInfo = navigation.PropertyInfo
						}
					})
					.Concat(entityType.GetSkipNavigations()
							.Where(skipNavigation => skipNavigation.PropertyInfo != null)
					.Select(skipNavigation => new
					{
						TypePropertyName = new TypePropertyName
						{
							Type = entityType.ClrType,
							PropertyName = skipNavigation.Name
						},
						NavigationTarget = new NavigationTarget
						{
							TargetClrType = skipNavigation.TargetEntityType.ClrType,
							NavigationType = GetNavigationType(skipNavigation),
							PropertyInfo = skipNavigation.PropertyInfo
						}
					})))
				.ToFrozenDictionary(a => a.TypePropertyName, a => a.NavigationTarget)
		};
	}

	private NavigationType GetNavigationType(IReadOnlyNavigation navigation)
	{
		if (navigation.IsOnDependent && (navigation.ForeignKey.DeclaringEntityType == navigation.DeclaringType))
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
