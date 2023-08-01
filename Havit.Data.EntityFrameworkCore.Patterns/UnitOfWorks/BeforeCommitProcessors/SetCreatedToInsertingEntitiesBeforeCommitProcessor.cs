using System.Reflection;
using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Během operace Insert nad entitou, která má vlastnost Created typu DateTime s hodnotou default(DateTime),
/// nastaví hodnotu této vlastnosti na aktuální datum/čas.
/// </summary>
public class SetCreatedToInsertingEntitiesBeforeCommitProcessor : IBeforeCommitProcessor<object>
{
	private readonly ITimeService timeService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public SetCreatedToInsertingEntitiesBeforeCommitProcessor(ITimeService timeService)
	{
		this.timeService = timeService;
	}

	/// <summary>
	/// Pro změnu Insert, pokud má entita vlastnost Created typu DateTime s hodnotou default(DateTime), nastaví hodnotu této vlastnosti na aktuální datum/čas.
	/// </summary>
	public void Run(ChangeType changeType, object changingEntity)
	{
		if (changeType != ChangeType.Insert)
		{
			return;
		}

		PropertyInfo createdProperty = changingEntity.GetType().GetProperty("Created");
		if ((createdProperty != null) && (createdProperty.PropertyType == typeof(DateTime)))
		{
			DateTime created = (DateTime)createdProperty.GetValue(changingEntity);
			if (created == default(DateTime))
			{
				createdProperty.SetValue(changingEntity, timeService.GetCurrentTime());
			}
		}
	}
}
