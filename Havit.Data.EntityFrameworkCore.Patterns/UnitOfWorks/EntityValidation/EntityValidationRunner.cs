using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation
{
	/// <summary>
	/// Spouští registrované entity validátory.	
	/// </summary>
	/// <remarks>
	/// Řeší logiku IEntityValidatorFactory tak, že pro každou entitu požaduje processory i pro bázové typy (až k IEntityValidatorFactory&lt;object&gt;).
	/// </remarks>
	public class EntityValidationRunner : IEntityValidationRunner
	{
		private readonly IEntityValidatorsFactory entityValidatorsFactory;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EntityValidationRunner(IEntityValidatorsFactory entityValidatorsFactory)
		{
			this.entityValidatorsFactory = entityValidatorsFactory;
		}

		/// <summary>
		/// Spustí IBeforeCommitProcessory pro zadané změny.
		/// </summary>
		public void Validate(Changes changes)
		{
			// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
			// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.
			
			var changeGroups = changes.Inserts.Select(item => new { Change = ChangeType.Insert, Entity = item })
				.Concat(changes.Updates.Select(item => new { Change = ChangeType.Update, Entity = item }))
				.Concat(changes.Deletes.Select(item => new { Change = ChangeType.Delete, Entity = item }))
				.GroupBy(item => item.Entity.GetType(), (type, groupChanges) => new { Type = type, Changes = groupChanges })
				.ToList();

			List<string> allValidationErrors = new List<string>();
			foreach (var changeGroup in changeGroups)
			{
				List<object> supportedValidators = new List<object>();
				// předpokládáme použití s Castle Windsor. Jeho factory (z pochopitelných důvodů) pri IBeforeCommitProcessor<Entity> neresolvuje processor pro případného předka, např. náš SetCreatedToInsertingEntitiesBeforeCommitProcessor pro IBeforeCommitProcessor<Entity>.
				Type type = changeGroup.Type;
				while (type != null)
				{
					supportedValidators.AddRange((IEnumerable<object>)entityValidatorsFactory.GetType().GetMethod(nameof(IEntityValidatorsFactory.Create)).MakeGenericMethod(type).Invoke(entityValidatorsFactory, null));
					type = type.BaseType;
				}

				Type entityValidatorType = typeof(IEntityValidator<>).MakeGenericType(changeGroup.Type);
				MethodInfo runMethod = entityValidatorType.GetMethod(nameof(IEntityValidator<object>.Validate));
				foreach (var change in changeGroup.Changes)
				{
					supportedValidators.ForEach(processor =>
					{
						IEnumerable<string> entityValidationErrors = (IEnumerable<string>)runMethod.Invoke(processor, new object[] { change.Change, change.Entity });
						allValidationErrors.AddRange(entityValidationErrors);
					});
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
		protected virtual void NotifyValidationErrors(IEnumerable<string> validationErrors)
		{
			throw new ValidationFailedException(validationErrors);
		}
	}
}
