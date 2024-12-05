using System;
using Microsoft.CodeAnalysis;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal class ServiceRegistrationEntry
{
	public INamedTypeSymbol[] ServiceTypes { get; set; }
	public INamedTypeSymbol ImplementationType { get; set; }
	public string Profile { get; set; }
	public string Lifetime { get; set; }
}
