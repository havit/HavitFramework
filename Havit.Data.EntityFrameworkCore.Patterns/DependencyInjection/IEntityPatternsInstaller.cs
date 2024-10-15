using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

// TODO: Odstranit
/// <summary>
/// Installer Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public interface IEntityPatternsInstaller
{
	/// <summary>
	/// Registruje do DI containeru DbContext vč. IDbContextFactory.
	/// </summary>
	public IEntityPatternsInstaller AddDbContext<TDbContext>()
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext;

	/// <summary>
	/// Registruje do DI containeru DbContext vč. IDbContextFactory.
	/// </summary>
	public IEntityPatternsInstaller AddDbContext<TDbContext>(Action<DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext;

	/// <summary>
	/// Registruje do DI containeru DbContext vč. IDbContextFactory.
	/// </summary>
	IEntityPatternsInstaller AddDbContext<TDbContext>(Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : DbContext, IDbContext;

	/// <summary>
	/// Registruje do DI containeru DbContext s DbContext poolingem vč. IDbContextFactory.
	/// </summary>
	public IEntityPatternsInstaller AddDbContextPool<TDbContext>(Action<DbContextOptionsBuilder> optionsAction, int poolSize = DbContextPool<DbContext>.DefaultPoolSize)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext;

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
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	IEntityPatternsInstaller AddLookupService<TService, TImplementation, TLookupDataInvalidationService>()
		where TService : class
		where TImplementation : class, TService
		where TLookupDataInvalidationService : ILookupDataInvalidationService;

	/// <summary>
	/// Registruje do DI containeru služby HFW pro Entity Framework Core.
	/// </summary>
	IEntityPatternsInstaller AddEntityPatterns();

	/// <summary>
	/// Registruje do DI containeru třídy z assembly předané v parametru dataLayerAssembly.
	/// Registrují se data seedy, data sources, repositories a data entries.
	/// </summary>
	IEntityPatternsInstaller AddDataLayer(Assembly assembly);

	/// <summary>
	/// Registruje do DI containeru dataseeds z dané assembly.
	/// </summary>
	IEntityPatternsInstaller AddDataSeeds(Assembly dataSeedsAssembly);
}