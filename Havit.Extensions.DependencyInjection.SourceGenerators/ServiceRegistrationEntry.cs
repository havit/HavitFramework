using System;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal class ServiceRegistrationEntry
{
	public string[] ServiceTypes { get; set; }
	public string ImplementationType { get; set; }
	public string Profile { get; set; }
	public string Lifetime { get; set; }

	public string GetCode()
	{
		return $"services.Add{Lifetime}<{String.Join("+", ServiceTypes)}, {ImplementationType}>(); // {Profile}";
	}
}
