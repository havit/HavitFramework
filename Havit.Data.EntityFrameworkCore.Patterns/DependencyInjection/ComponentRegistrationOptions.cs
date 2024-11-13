using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

/// <summary>
/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public class ComponentRegistrationOptions
{
	/// <summary>
	/// Typ použitého UnitOfWork.
	/// Výchozí hodnota je DbUnitOfWork.
	/// </summary>
	public Type UnitOfWorkType { get; set; }

	/// <summary>
	/// Installer služeb pro cachování. Výchozí hodnotou je instance DefaultCachingInstalleru.
	/// </summary>
	public ICachingInstaller CachingInstaller { get; set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	internal ComponentRegistrationOptions()
	{
		CachingInstaller = new DefaultCachingInstaller();
	}



}
