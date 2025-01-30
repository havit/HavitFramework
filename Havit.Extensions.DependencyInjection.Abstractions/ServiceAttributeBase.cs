using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Abstractions;

/// <summary>
/// Slouží k označení tříd, které mají být automaticky zaregistrovány do IoC containeru.
/// Bázová (abstraktní) třída.
/// </summary>
/// <remarks>
/// Při změně pozor na nutnost úpravy ve třídě ServiceAttributeContants.
/// </remarks>
public abstract class ServiceAttributeBase : Attribute
{
	/// <summary>
	/// Lifetime (lifestyle) s jakým má být služba zaregistrována.
	/// </summary>
	/// <remarks>
	/// Při změně pozor na nutnost úpravy ve třídě ServiceAttributeContants.
	/// </remarks>
	public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

	/// <summary>
	/// Název profilu. Při registraci jsou registrovány jen služby požadovaných profilů.
	/// </summary>
	/// <remarks>
	/// Při změně pozor na nutnost úpravy ve třídě ServiceAttributeContants.
	/// </remarks>
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