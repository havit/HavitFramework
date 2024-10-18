using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;

/// <summary>
/// Spouští registrované entity validátory.	
/// </summary>
/// <remarks>
/// Řeší logiku IEntityValidatorFactory tak, že pro každou entitu požaduje processory i pro bázové typy (až k IEntityValidatorFactory&lt;object&gt;).
/// </remarks>
public class EntityValidationRunner : IEntityValidationRunner
{
	private readonly IEntityValidatorsFactory _entityValidatorsFactory;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityValidationRunner(IEntityValidatorsFactory entityValidatorsFactory)
	{
		_entityValidatorsFactory = entityValidatorsFactory;
	}

	/// <summary>
	/// Spustí IBeforeCommitProcessory pro zadané změny.
	/// </summary>
	public void Validate(Changes changes)
	{
		// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
		// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.

		var changesGroups = changes
			.Where(change => change.ClrType != null)
			.GroupBy(change => change.ClrType)
			.ToList();

		object[] runMethodParameters = new object[2];
		List<string> allValidationErrors = new List<string>();
		foreach (var changesGroup in changesGroups)
		{
			List<object> supportedValidators = new List<object>();
			// Factory pro IEntityValidators<Entity> nevrací processory pro případné předky, musíme proto zajistit zde podporu pro before commitprocessory předků.
			Type type = changesGroup.Key;
			while (type != null)
			{
				supportedValidators.AddRange((IEnumerable<object>)_entityValidatorsFactory.GetType().GetMethod(nameof(IEntityValidatorsFactory.Create)).MakeGenericMethod(type).Invoke(_entityValidatorsFactory, null));
				type = type.BaseType;
			}

			Type entityValidatorType = typeof(IEntityValidator<>).MakeGenericType(changesGroup.Key);
			MethodInfo runMethod = entityValidatorType.GetMethod(nameof(IEntityValidator<object>.Validate));
			foreach (var change in changesGroup)
			{
				runMethodParameters[0] = change.ChangeType;
				runMethodParameters[1] = change.Entity;
				foreach (var supportedValidator in supportedValidators)
				{
					IEnumerable<string> entityValidationErrors = (IEnumerable<string>)runMethod.Invoke(supportedValidator, runMethodParameters);
					allValidationErrors.AddRange(entityValidationErrors);
				};
			}
		}

		if (allValidationErrors.Any())
		{
			NotifyValidationErrors(allValidationErrors);
		}
	}

	/// <summary>
	/// Oznámí validační chyby.
	/// Vyhazuje ValidationFailedException.
	/// </summary>
	protected virtual void NotifyValidationErrors(List<string> validationErrors)
	{
		throw new ValidationFailedException(validationErrors);
	}
}
