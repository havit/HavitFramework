using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Installer Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	public interface IEntityPatternsInstaller
	{
		/// <summary>
		/// Registruje do DI containeru DbContext a související.
		/// </summary>
		IEntityPatternsInstaller AddDbContext<TDbContext>(DbContextOptions options = null)
			where TDbContext : class, IDbContext;

		/// <summary>
		/// Registruje do DI containeru služby pro lokalizaci.
		/// </summary>
		IEntityPatternsInstaller AddLocalizationServices<TLanguage>()
			where TLanguage : class, Havit.Model.Localizations.ILanguage;

		/// <summary>
		/// Zaregistruje do DI containeru lookup službu.
		/// </summary>
		IEntityPatternsInstaller AddLookupService<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService, ILookupDataInvalidationService;

		/// <summary>
		/// Registruje do DI containeru služby HFW pro Entity Framework Core.
		/// </summary>
		IEntityPatternsInstaller AddEntityPatterns();

		/// <summary>
		/// Registruje do DI containeru třídy z assembly předané v parametru dataLayerAssembly.
		/// Registrují se data seedy, data sources, repositories a data entries.
		/// </summary>
		IEntityPatternsInstaller AddDataLayer(Assembly assembly);
	}
}