﻿using System.Reflection;

namespace Havit.Data.Entity.Patterns.Windsor.Installers;

/// <summary>
/// Installer Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public interface IEntityPatternsInstaller
{
	/// <summary>
	/// Registruje do windsor containeru:
	/// <list type="bullet">
	///		<item>
	///			<term>IDbContext</term>
	///			<description>Jako předaný TDbContext. Lifestyle je určen dle options installeru (DbContextLifestyle).</description>
	///		</item>
	/// </list>
	/// </summary>
	IEntityPatternsInstaller RegisterDbContext<TDbContext>()
		where TDbContext : class, IDbContext;

	/// <summary>
	/// Registruje do windsor containeru:
	/// <list type="bullet">
	///		<item>
	///			<term>ILanguageService</term>
	///			<description>
	///				Jako předaný ILanguageService pro předaný TLanguage (DbLanguageService&lt;TLanguage&gt;).
	///				Lifestyle je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>ILocalizationService</term>
	///			<description>
	///				LocalizationService. 
	///				Lifestyle je singleton.
	///			</description>
	///		</item>
	/// </list>
	/// </summary>
	IEntityPatternsInstaller RegisterLocalizationServices<TLanguage>()
		where TLanguage : class, Havit.Model.Localizations.ILanguage;

	/// <summary>
	/// Registruje do windsor containeru:
	/// <list type="bullet">
	///		<item>
	///			<term>ISoftDeleteManager</term>
	///			<description>
	///				SoftDeleteManager.
	///				Lifestyle je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IDataEntrySymbolService&lt;&gt;</term>
	///			<description>
	///				DbDataEntrySymbolService&lt;&gt;
	///				Lifestyle je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>ICurrentCultureService</term>
	///			<description>
	///				CurrentCultureService.
	///				Lifestyle je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IDataSeedRunner</term>
	///			<description>
	///				DataSeedRunner.
	///				Lifestyle je transient.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IDataSeedPersister</term>
	///			<description>
	///				DbDataSeedPersister.
	///				Lifestyle je transient.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IDataSourceFactory&lt;&gt;</term>
	///			<description>
	///				Factory pro IDataSource&lt;&gt;.
	///				Lifestyle factory je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IRepositoryFactory&lt;&gt;</term>
	///			<description>
	///				Factory pro IRepository&lt;&gt;.
	///				Lifestyle factory je singleton.
	///			</description>
	///		</item>
	///		<item>
	///			<term>IUnitOfWork</term>
	///			<description>
	///				Registruje typ dle options installery (UnitOfWorkType).
	///				Lifestyle je určen dle options installeru (UnitOfWorkLifestyle).
	///			</description>
	///		</item>
	///		<item>
	///			<term>IDataLoader</term>
	///			<description>
	///				DbDataLoaderWithLoadedPropertiesMemory.
	///				Lifestyle je určen dle options installeru (DataLoaderLifestyle).
	///			</description>
	///		</item>
	/// </list>
	/// </summary>
	IEntityPatternsInstaller RegisterEntityPatterns();

	/// <summary>
	/// Registruje do windsor containeru třídy z assembly předané v parametru dataLayerAssembly:
	/// <list type="bullet">
	///		<item>
	///			<term>třídy implementující IDataSeed</term>
	///			<description>
	///				Registruje je pod IDataSeed.
	///				Lifestyle transient.
	///			</description>
	///		</item>
	///		<item>
	///			<term>potomky (implementace) DbDataSource&lt;&gt;</term>
	///			<description>
	///				Registruje je pod IDataSource&lt;TEntity&gt; a IEntityDataSource. TEntity je skutečný typ entity.
	///				Lifestyle je transient.
	///			</description>
	///		</item>
	///		<item>
	///			<term>potomky (implementace) DbRepository&lt;&gt;</term>
	///			<description>
	///				Registruje je pod IRepository&lt;TEntity&gt; a IEntityRepository. TEntity je skutečný typ entity.
	///				Lifestyle je určen dle options installeru (RepositoriesLifestyle).
	///			</description>
	///		</item>
	///		<item>
	///			<term>implementující IDataEntries&lt;&gt;</term>
	///			<description>
	///				Registruje je pod IDataSeed.
	///				Lifestyle je určen dle options installeru (DataEntriesLifestyle).
	///			</description>
	///		</item>
	/// </list>
	/// <remarks>
	///		Nikdy neregistruje služby (třídy), které jsou dekorovány atributem FakeAttribute či které jsou abstraktní.
	/// </remarks>
	/// </summary>
	IEntityPatternsInstaller RegisterDataLayer(Assembly assembly);
}