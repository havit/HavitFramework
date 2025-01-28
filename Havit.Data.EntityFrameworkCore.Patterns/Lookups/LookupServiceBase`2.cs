using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Bázová třída pro vyhledávání entit dle klíče (jednoduchého i složeného) pro entity s primárním klíčem typu System.Int32.
/// Použití:
/// 1) Podědit od této třídy a implementovat abstrakční vlastností
/// 2) Eventuelně nakonfigurovat chování overridováním virtuálních vlastností.
/// 3) Implementovat nějaký vlastní interface, imlementace bude volat GetEntityByLookupKey (ev. GetEntityKeyByLookupKey).
/// </summary>
/// <typeparam name="TLookupKey">Typ klíče.</typeparam>
/// <typeparam name="TEntity">Entita, kterou hledáme.</typeparam>
/// <remarks>
/// Určeno k implementaci občasně měněných entit, ev. entit které se mění hromadně (naráz).
/// Není garantována stoprocentní spolehlivost u entit, které se mění často (myšleno zejména paralelně) v různých transakcích - invalidace a aktualizace může proběhnout v jiném pořadí, než v jakém doběhly commity.
/// </remarks>
public abstract class LookupServiceBase<TLookupKey, TEntity> : LookupServiceBase<TLookupKey, TEntity, int>
	where TEntity : class
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected LookupServiceBase(IEntityLookupDataStorage lookupStorage, IRepository<TEntity, int> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager)
		: base(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager, null)
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected LookupServiceBase(IEntityLookupDataStorage lookupStorage, IRepository<TEntity, int> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager, IDistributedLookupDataInvalidationService distributedLookupDataInvalidationService)
		: base(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager, distributedLookupDataInvalidationService)
	{
	}
}
