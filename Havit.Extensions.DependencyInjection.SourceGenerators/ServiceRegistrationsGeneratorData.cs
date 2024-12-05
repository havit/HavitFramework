using System.Collections.Immutable;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal class ServiceRegistrationsGeneratorData
{
	public BuildConfiguration BuildConfiguration { get; set; }
	public ImmutableArray<ServiceRegistrationEntry> ServiceRegistrationEntries { get; set; }
}
