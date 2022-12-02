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
	public const string DefaultProfile = "@DefaultProfile";

	/// <summary>
	/// Typ, pod kterým bude služba zaregistrována.
	/// Nelze kombinovat s <see cref="ServiceTypes"/>.
	/// </summary>
#if NET7_0_OR_GREATER
	[Obsolete("Use a generic variant of the ServiceAttribute.")]
#endif
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
#if NET7_0_OR_GREATER
	[Obsolete("Use a generic variant of the ServiceAttribute.")]
#endif
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
#pragma warning disable CS0618
			throw new InvalidOperationException($"Properties {nameof(ServiceType)} and {nameof(ServiceTypes)} are mutual exclusive. Use {nameof(ServiceType)} or {nameof(ServiceTypes)}, not both.");
#pragma warning restore CS0618
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