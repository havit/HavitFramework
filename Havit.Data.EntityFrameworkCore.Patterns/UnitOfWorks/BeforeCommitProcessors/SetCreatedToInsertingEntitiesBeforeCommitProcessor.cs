using System.Collections.Concurrent;
using System.Reflection;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Během operace Insert nad entitou, která má vlastnost Created typu DateTime s hodnotou default(DateTime),
/// nastaví hodnotu této vlastnosti na aktuální datum/čas.
/// </summary>
public class SetCreatedToInsertingEntitiesBeforeCommitProcessor : BeforeCommitProcessor<object>
{
	private readonly ITimeService _timeService;
	private readonly ConcurrentDictionary<Type, PropertyInfo> _createdProperties = new ConcurrentDictionary<Type, PropertyInfo>();

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public SetCreatedToInsertingEntitiesBeforeCommitProcessor(ITimeService timeService)
	{
		_timeService = timeService;
	}

	/// <summary>
	/// Pro změnu Insert, pokud má entita vlastnost Created typu DateTime s hodnotou default(DateTime), nastaví hodnotu této vlastnosti na aktuální datum/čas.
	/// </summary>
	public override ChangeTrackerImpact Run(ChangeType changeType, object changingEntity)
	{
		if (changeType == ChangeType.Insert)
		{
			PropertyInfo createdProperty = _createdProperties.GetOrAdd(
				changingEntity.GetType(),
				type =>
				{
					var propertyInfo = type.GetProperty("Created");
					if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime)) // propertyInfo nás zajímá pouze pokud je typu DateTime
					{
						return propertyInfo;
					}
					return null;
				});

			if (createdProperty != null)
			{
				DateTime created = (DateTime)createdProperty.GetValue(changingEntity);
				if (created == default(DateTime))
				{
					createdProperty.SetValue(changingEntity, _timeService.GetCurrentTime());
				}
			}
		}

		return ChangeTrackerImpact.NoImpact;
	}
}
