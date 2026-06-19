using System.Collections.Concurrent;
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

	// Cache IEntityValidator<TEntity>.Validate metody dle CLR typu entity - vyhneme se opakovanému MakeGenericType + GetMethod.
	// Díky kontravarianci IEntityValidator<in TEntity> je metoda získaná pro typ skupiny použitelná i pro validátory předků.
	private static readonly ConcurrentDictionary<Type, MethodInfo> _validateMethodsByEntityType = new ConcurrentDictionary<Type, MethodInfo>();

	// Cache uzavřené generické IEntityValidatorsFactory.Create<TEntity>() metody dle CLR typu entity - vyhneme se opakovanému GetMethod + MakeGenericMethod.
	// Metoda je brána z rozhraní (ne z konkrétního typu factory), takže je nezávislá na instanci a invoke se virtuálně nadispatchuje na implementaci.
	private static readonly MethodInfo _createValidatorsMethodDefinition = typeof(IEntityValidatorsFactory).GetMethod(nameof(IEntityValidatorsFactory.Create));
	private static readonly ConcurrentDictionary<Type, MethodInfo> _createValidatorsMethodsByEntityType = new ConcurrentDictionary<Type, MethodInfo>();

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

		ILookup<Type, Change> changesGroups = changes.GetChangesByClrType();

		object[] runMethodParameters = new object[2];
		List<string> allValidationErrors = new List<string>();
		foreach (IGrouping<Type, Change> changesGroup in changesGroups)
		{
			List<object> supportedValidators = new List<object>();
			// Factory pro IEntityValidators<Entity> nevrací processory pro případné předky, musíme proto zajistit zde podporu pro before commitprocessory předků.
			Type type = changesGroup.Key;
			while (type != null)
			{
				MethodInfo createMethod = _createValidatorsMethodsByEntityType.GetOrAdd(
					type,
					static t => _createValidatorsMethodDefinition.MakeGenericMethod(t));
				supportedValidators.AddRange((IEnumerable<object>)createMethod.Invoke(_entityValidatorsFactory, null));
				type = type.BaseType;
			}

			MethodInfo runMethod = _validateMethodsByEntityType.GetOrAdd(
				changesGroup.Key,
				static type => typeof(IEntityValidator<>).MakeGenericType(type).GetMethod(nameof(IEntityValidator<object>.Validate)));
			foreach (Change change in changesGroup)
			{
				runMethodParameters[0] = change.ChangeType;
				runMethodParameters[1] = change.Entity;
				foreach (var supportedValidator in supportedValidators)
				{
					IEnumerable<string> entityValidationErrors = (IEnumerable<string>)runMethod.Invoke(supportedValidator, runMethodParameters);
					allValidationErrors.AddRange(entityValidationErrors);
				}
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
