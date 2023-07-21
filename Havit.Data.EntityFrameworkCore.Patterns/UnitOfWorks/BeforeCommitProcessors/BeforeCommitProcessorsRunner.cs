using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

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

		var changeGroups = changes
			.Where(change => change.ClrType != null)
			.GroupBy(change => change.ClrType, (entityType, entityTypeChanges) => new { Type = entityType, Changes = entityTypeChanges })
			.ToList();

		foreach (var changeGroup in changeGroups)
		{
			List<object> supportedProcessors = new List<object>();
			// Factory pro IBeforeCommitProcessor<Entity> nevrací processory pro případné předky, musíme proto zajistit zde podporu pro before commitprocessory předků.
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
				supportedProcessors.ForEach(processor => runMethod.Invoke(processor, new object[] { change.ChangeType, change.Entity }));
			}
		}
	}
}
