using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Havit.Extensions.DependencyInjection.Abstractions;

public abstract class ServiceAttributeBase : Attribute
{
	/// <summary>
	/// Lifetime (lifestyle) s jakým má být služba zaregistrována.
	/// </summary>
	public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

	/// <summary>
	/// Název profilu. Při registraci jsou registrovány jen služby požadovaných profilů.
	/// </summary>
	public string Profile { get; set; } = ServiceAttribute.DefaultProfile;

	/// <summary>
	/// Vrátí služby, pod které má být služba zaregistrována.
	/// </summary>
	public abstract Type[] GetServiceTypes();

	/// <inheritdoc />
	public override string ToString()
	{
		return $"{nameof(Profile)} = \"{Profile}\", ServiceTypes = {{ {String.Join(", ", GetServiceTypes().Select(type => type.FullName))} }}, Lifetime = {Lifetime}";
	}
}