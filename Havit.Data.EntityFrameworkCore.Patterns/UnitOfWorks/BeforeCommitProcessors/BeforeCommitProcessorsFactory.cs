using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

internal class BeforeCommitProcessorsFactory : IBeforeCommitProcessorsFactory
{
	private readonly IServiceProvider _serviceProvider;

	public BeforeCommitProcessorsFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IEnumerable<IBeforeCommitProcessorInternal> Create(Type entityType)
	{
		ArgumentNullException.ThrowIfNull(entityType);

		return CreateInternal(entityType);
	}

	private IEnumerable<IBeforeCommitProcessorInternal> CreateInternal(Type entityType)
	{
		if (entityType.BaseType != null)
		{
			foreach (IBeforeCommitProcessorInternal beforeCommitProcessor in CreateInternal(entityType.BaseType))
			{
				yield return beforeCommitProcessor;
			}
		}

		// reflexe vytváří typ IEnumerable<IBeforeCommitProcessor<EntityType>>
		Type beforeCommitProcessorsType = typeof(IEnumerable<>).MakeGenericType(typeof(IBeforeCommitProcessor<>).MakeGenericType(entityType));
		IEnumerable<IBeforeCommitProcessorInternal> beforeCommitProcessors = (IEnumerable<IBeforeCommitProcessorInternal>)_serviceProvider.GetRequiredService(beforeCommitProcessorsType);
		foreach (IBeforeCommitProcessorInternal beforeCommitProcessor in beforeCommitProcessors)
		{
			yield return beforeCommitProcessor;
		}
	}

}
