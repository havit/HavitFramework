using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors
{
	/// <summary>
	/// Spouští registrované IBeforeCommitProcessory.	
	/// </summary>
	/// <remarks>
	/// Řeší logiku IBeforeCommitProcessorsFactory tak, že pro každou entitu požaduje processory i pro bázové typy (až k IBeforeCommitProcessor&lt;object&gt;).
	/// </remarks>
	public class BeforeCommitProcessorsRunner : IBeforeCommitProcessorsRunner
	{
		private readonly IBeforeCommitProcessorsFactory beforeCommitProcessorsFactory;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public BeforeCommitProcessorsRunner(IBeforeCommitProcessorsFactory beforeCommitProcessorsFactory)
		{
			this.beforeCommitProcessorsFactory = beforeCommitProcessorsFactory;
		}

		/// <summary>
		/// Spustí IBeforeCommitProcessory pro zadané změny.
		/// </summary>
		public void Run(Changes changes)
		{
			// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
			// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.

			var changeGroups = changes.Inserts.Select(item => new { Change = ChangeType.Insert, Entity = item })
				.Concat(changes.Updates.Select(item => new { Change = ChangeType.Update, Entity = item }))
				.Concat(changes.Deletes.Select(item => new { Change = ChangeType.Delete, Entity = item }))
				.GroupBy(item => item.Entity.GetType(), (type, groupChanges) => new { Type = type, Changes = groupChanges })
				.ToList();

			foreach (var changeGroup in changeGroups)
			{
				List<object> supportedProcessors = new List<object>();
				// předpokládáme použití s Castle Windsor. Jeho factory (z pochopitelných důvodů) pri IBeforeCommitProcessor<Entity> neresolvuje processor pro případného předka, např. náš SetCreatedToInsertingEntitiesBeforeCommitProcessor pro IBeforeCommitProcessor<Entity>.
				Type type = changeGroup.Type;
				while (type != null)
				{
					supportedProcessors.AddRange((IEnumerable<object>)beforeCommitProcessorsFactory.GetType().GetMethod(nameof(IBeforeCommitProcessorsFactory.Create)).MakeGenericMethod(type).Invoke(beforeCommitProcessorsFactory, null));
					type = type.BaseType;
				}

				Type beforeCommitProcessorType = typeof(IBeforeCommitProcessor<>).MakeGenericType(changeGroup.Type);
				MethodInfo runMethod = beforeCommitProcessorType.GetMethod(nameof(IBeforeCommitProcessor<object>.Run));
				foreach (var change in changeGroup.Changes)
				{
					supportedProcessors.ForEach(processor => runMethod.Invoke(processor, new object[] { change.Change, change.Entity }));
				}
			}
		}
	}
}
