using System;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Analyzers;

internal class ServiceRegistrationEntry
{
	public string[] ServiceTypes { get; set; }
	public string ImplementationType { get; set; }
	public string Profile { get; set; }
	public ServiceLifetime Lifetime { get; set; }

	public string GetCode()
	{
		return $"services.Add{Lifetime}<{String.Join("+", ServiceTypes)}, {ImplementationType}>(); // {Profile}";
	}
}
