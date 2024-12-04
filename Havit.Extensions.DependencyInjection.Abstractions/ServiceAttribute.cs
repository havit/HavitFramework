using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Abstractions;

/// <summary>
/// Slouží k označení tříd, které mají být automaticky zaregistrovány do IoC containeru.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ServiceAttribute : ServiceAttributeBase
{
	/// <summary>
	/// Výchozí název profilu (pokud není specifikován).
	/// </summary>
	/// <remarks>
	/// Při změně pozor na nutnost úpravy ve třídě ServiceAttributeContants.
	/// </remarks>
	public const string DefaultProfile = "@DefaultProfile";

	/// <summary>
	/// Typ, pod kterým bude služba zaregistrována.
	/// Nelze kombinovat s <see cref="ServiceTypes"/>.
	/// </summary>
	/// <remarks>
	/// ServiceType/ServiceTypes potřebujeme i pro .NET 7, protože bez nich nemůžeme použít open generic types.
	/// </remarks>
	public Type ServiceType
	{
		get
		{
			return _serviceType;
		}
		set
		{
			_serviceType = value;
			CheckServiceTypeAndServiceTypes();
		}
	}

	private Type _serviceType;

	/// <summary>
	/// Typy, pod kterým bude služba zaregistrována.
	/// Nelze kombinovat s <see cref="ServiceType"/>.
	/// </summary>
	/// <remarks>
	/// ServiceType/ServiceTypes potřebujeme i pro .NET 7, protože bez nich nemůžeme použít open generic types.
	/// </remarks>
	public Type[] ServiceTypes
	{
		get
		{
			return _serviceTypes;
		}
		set
		{
			if ((value != null) && (value.Length == 0))
			{
				throw new ArgumentException($"Cannot set an empty array to {nameof(ServiceTypes)} property.");
			}

			_serviceTypes = value;
			CheckServiceTypeAndServiceTypes();
		}
	}
	private Type[] _serviceTypes;

	private void CheckServiceTypeAndServiceTypes()
	{
		if ((_serviceType != null) && (_serviceTypes != null))
		{
			throw new InvalidOperationException($"Properties {nameof(ServiceType)} and {nameof(ServiceTypes)} are mutual exclusive. Use {nameof(ServiceType)} or {nameof(ServiceTypes)}, not both.");
		}
	}

	/// <summary>
	/// Vrátí služby, pod které má být služba zaregistrována.
	/// </summary>
	public override Type[] GetServiceTypes()
	{
		if (_serviceType != null)
		{
			return new Type[] { _serviceType };
		}

		return _serviceTypes; // can be null
	}
}