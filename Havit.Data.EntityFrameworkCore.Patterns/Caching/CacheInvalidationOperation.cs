namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Reprezentuje operaci invalidace cache.
/// Důvodem existence této třídy je, že chceme naplánovat provedení invalidace cache ještě před samotným uložením dat do databáze (DbContext.SaveChanges).
/// Provedení invalidace je pak reprezentováno touto třídou, přičemž metoda Invalidate této třídy je zavolána po uložení změn do databáze.
/// </summary>
public class CacheInvalidationOperation
{
	private readonly Action invalidateAction;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CacheInvalidationOperation(Action invalidateAction)
	{
		this.invalidateAction = invalidateAction;
	}

	/// <summary>
	/// Provede naplánovanou invalidaci dat v cache.
	/// </summary>
	public void Invalidate()
	{
		invalidateAction.Invoke();
	}
}
