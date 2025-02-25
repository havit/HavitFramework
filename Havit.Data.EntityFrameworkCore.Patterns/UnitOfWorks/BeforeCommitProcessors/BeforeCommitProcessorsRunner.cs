using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;
using Havit.Data.Patterns.UnitOfWorks;

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
	/// Spustí IBeforeCommitProcessory pro zadané změny. Bez podpory pro asynchronní before commit procesory.
	/// </summary>
	public ChangeTrackerImpact Run(Changes changes)
	{
		ChangeTrackerImpact result = ChangeTrackerImpact.NoImpact;

		// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
		// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.
		ILookup<Type, Change> changesGroups = changes.GetChangesByClrType();

		foreach (IGrouping<Type, Change> changesGroup in changesGroups)
		{
			IEnumerable<IBeforeCommitProcessorInternal> supportedProcessors = _beforeCommitProcessorsFactory.Create(changesGroup.Key /* entity type*/);

			foreach (var supportedProcessor in supportedProcessors)
			{
				foreach (Change change in changesGroup)
				{
					if (supportedProcessor.Run(change.ChangeType, change.Entity) == ChangeTrackerImpact.StateChanged)
					{
						result = ChangeTrackerImpact.StateChanged;
					}

					ValueTask<ChangeTrackerImpact> task = supportedProcessor.RunAsync(change.ChangeType, change.Entity);
					if (!task.IsCompleted)
					{
						throw new InvalidOperationException($"Async before commit processors are supported only for async {nameof(IUnitOfWork)}.{nameof(IUnitOfWork.CommitAsync)} method.");
					}

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
					ChangeTrackerImpact changeTrackerImpact = task.GetAwaiter().GetResult(); // pro propagaci případných výjimek
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
					if (changeTrackerImpact == ChangeTrackerImpact.StateChanged)
					{
						result = ChangeTrackerImpact.StateChanged;
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Spustí IBeforeCommitProcessory pro zadané změny.
	/// </summary>
	public async ValueTask<ChangeTrackerImpact> RunAsync(Changes changes, CancellationToken cancellationToken = default)
	{
		ChangeTrackerImpact result = ChangeTrackerImpact.NoImpact;

		// z výkonových důvodů - omezení procházení pole processorů - seskupíme objekty podle typu,
		// vyhledáme procesor pro daný typ a spustíme jej nad všemi objekty ve skupině.
		ILookup<Type, Change> changesGroups = changes.GetChangesByClrType();

		foreach (IGrouping<Type, Change> changesGroup in changesGroups)
		{
			IEnumerable<IBeforeCommitProcessorInternal> supportedProcessors = _beforeCommitProcessorsFactory.Create(changesGroup.Key /* entity type*/);

			foreach (var supportedProcessor in supportedProcessors)
			{
				foreach (Change change in changesGroup)
				{
					if (supportedProcessor.Run(change.ChangeType, change.Entity) == ChangeTrackerImpact.StateChanged)
					{
						result = ChangeTrackerImpact.StateChanged;
					}

					if ((await supportedProcessor.RunAsync(change.ChangeType, change.Entity, cancellationToken).ConfigureAwait(false)) == ChangeTrackerImpact.StateChanged)
					{
						result = ChangeTrackerImpact.StateChanged;
					}
				}
			}
		}

		return result;
	}
}
