﻿namespace Havit.Extensions.DependencyInjection.Abstractions;

/// <summary>
/// Slouží k označení tříd, které mají být automaticky zaregistrovány do IoC containeru.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ServiceAttribute<TServiceType> : ServiceAttributeBase
{
	/// <summary>
	/// Vrátí služby, pod které má být služba zaregistrována.
	/// </summary>
	public override Type[] GetServiceTypes()
	{
		return new Type[] { typeof(TServiceType) };
	}
}
