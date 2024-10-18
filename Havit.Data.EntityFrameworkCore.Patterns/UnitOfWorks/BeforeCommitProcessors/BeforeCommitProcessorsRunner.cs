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
	private readonly IBeforeCommitProcessorsFactory _beforeCommitProcessorsFactory;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public BeforeCommitProcessorsRunner(IBeforeCommitProcessorsFactory beforeCommitProcessorsFactory)
	{
		_beforeCommitProcessorsFactory = beforeCommitProcessorsFactory;
	}

	/// <summary>
	/// Spustí IBeforeCommitProcessory pro zadané změny.
	/// </summary>
	public void Run(Changes changes)
	{
		// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
		// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.

		var changesGroups = changes
			.Where(change => change.ClrType != null)
			.GroupBy(change => change.ClrType)
			.ToList();

		var runMethodParameters = new object[2];

		foreach (var changesGroup in changesGroups)
		{
			List<object> supportedProcessors = new List<object>(4);
			// Factory pro IBeforeCommitProcessor<Entity> nevrací processory pro případné předky, musíme proto zajistit zde podporu pro before commitprocessory předků.
			Type type = changesGroup.Key;
			while (type != null)
			{
				supportedProcessors.AddRange((IEnumerable<object>)_beforeCommitProcessorsFactory.GetType().GetMethod(nameof(IBeforeCommitProcessorsFactory.Create)).MakeGenericMethod(type).Invoke(_beforeCommitProcessorsFactory, null));
				type = type.BaseType;
			}

			Type beforeCommitProcessorType = typeof(IBeforeCommitProcessor<>).MakeGenericType(changesGroup.Key);
			MethodInfo runMethod = beforeCommitProcessorType.GetMethod(nameof(IBeforeCommitProcessor<object>.Run));

			foreach (var change in changesGroup)
			{
				runMethodParameters[0] = change.ChangeType;
				runMethodParameters[1] = change.Entity;
				foreach (var supportedProcessor in supportedProcessors)
				{
					runMethod.Invoke(supportedProcessor, runMethodParameters);
				}
			}
		}
	}
}
