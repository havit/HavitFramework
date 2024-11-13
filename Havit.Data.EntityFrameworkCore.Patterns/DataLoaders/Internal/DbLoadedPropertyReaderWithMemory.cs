namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

/// <inheritdoc />
public class DbLoadedPropertyReaderWithMemory : DbLoadedPropertyReader
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbLoadedPropertyReaderWithMemory(IDbContext dbContext) : base(dbContext)
	{
		// NOOP
	}

	/// <summary>
	/// Vrací true, pokud je vlastnost objektu již načtena.
	/// Předpokládá, že každá vlastnost každého objektu, na který se ptáme, bude načtena. Proto si ji uloží do paměti a při opakovaném přístupu vrací true, pokud máme u daného objektu danou vlastnost zapamatovanou.
	/// Pro vlastnosti objektů, které nejsou v paměti, volá bázovou třídu.
	/// </summary>
	public override bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName)
	{
		LoadedEntityProperty loadedEntityProperty = new LoadedEntityProperty(entity, propertyName);
		return loadedEntityProperties.Add(loadedEntityProperty)
			? base.IsEntityPropertyLoaded(entity, propertyName) // klíč nebyl v kolekci -> vlastnost ještě nemusela být načtena
			: true; // klíč byl v kolekci, již jsme načítali
	}
	private readonly HashSet<LoadedEntityProperty> loadedEntityProperties = new HashSet<LoadedEntityProperty>();

	internal sealed record class LoadedEntityProperty(object Entity, string PropertyName);
}
