using System.Data.Entity;

namespace Havit.Data.Entity;

/// <summary>
/// Interface DbContextu.
/// </summary>
public interface IDbContext
{
	/// <summary>
	/// Zpřístupňuje Configuration.AutoDetectChangesEnabled.
	/// </summary>
	bool AutoDetectChangesEnabled { get; set; }

	/// <summary>
	/// Vrátí objekt pro přímý přístup k databázi.
	/// </summary>
	IDbContextDatabase Database { get; }

	/// <summary>
	/// Vrací DbSet pro danou entitu.
	/// </summary>
	DbSet<TEntity> Set<TEntity>()
		where TEntity : class;

	/// <summary>
	/// Vrací stav entity z change trackeru.
	///  Metoda primárně slouží pro možnost testovatelnosti (mockování). K dispozici je context.Entry(entity).State, avšak taková metoda nelze mockovat neboť
	/// a) DbEntityEntry nelze podědit, protože má interní konstruktor.
	/// b) DbEntityEntry.State není virtuální
	/// </summary>
	EntityState GetEntityState<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Uloží změny.
	/// </summary>
	void SaveChanges();

	/// <summary>
	/// Uloží změny.
	/// </summary>
	Task SaveChangesAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Registruje akci k provedení po save changes. Akce je provedena metodou AfterSaveChanges.
	/// </summary>
	void RegisterAfterSaveChangesAction(Action action);

	/// <summary>
	/// Vrátí objekty v daném stavu.
	/// </summary>
	object[] GetObjectsInState(EntityState state);

	/// <summary>
	/// Nastaví objekt do požadovaného stavu.
	/// </summary>
	void SetEntityState<TEntity>(TEntity entity, EntityState entityState)
		where TEntity : class;

	/// <summary>
	/// Volá DetectChanges na ChangeTrackeru.
	/// </summary>
	void DetectChanges();

	/// <summary>
	/// Vrací true, pokud je daná vlastnost na entitě načtena.
	/// </summary>
	bool IsEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName)
		where TEntity : class;

	/// <summary>
	/// Vrací true, pokud je daná vlastnost (kolekce) na entitě načtena.
	/// </summary>
	bool IsEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName)
		where TEntity : class;

	/// <summary>
	/// Nastaví informaci o tom, zda byla daná vlastnost dané entity načtena. Viz DbReferenceEntry.IsLoaded.
	/// </summary>
	void SetEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName, bool loadedValue)
		where TEntity : class;

	/// <summary>
	/// Nastaví informaci o tom, zda byla daná vlastnost dané entity načtena. Viz DbCollectionEntry.IsLoaded.
	/// </summary>
	void SetEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName, bool loadedValue)
	where TEntity : class;

	/// <summary>
	/// Provede akci s AutoDetectChangesEnabled nastaveným na false, přičemž je poté AutoDetectChangesEnabled nastaven na původní hodnotu.
	/// </summary>
	TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action);

	/// <summary>
	/// Provede akci s Configuration.UseDatabaseNullSemantics nastaveným na true, přičemž je poté Configuration.UseDatabaseNullSemantics nastaven na původní hodnotu.
	/// </summary>
	TResult ExecuteWithDatabaseNullSemantics<TResult>(Func<TResult> action);
}
