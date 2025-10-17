namespace Havit.Data.Patterns.UnitOfWorks;

/// <summary>
/// Unit of Work.
/// </summary>
public interface IUnitOfWork
{
	/// <summary>
	/// Uloží změny registrované v Unit of Work.
	/// </summary>
	void Commit();

	/// <summary>
	/// Asynchronně uloží změny registrované v Unit of Work.
	/// </summary>
	Task CommitAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// </summary>
	void AddForInsert<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// Určeno pro použití s entitami využívajícími asynchronní operace, typicky jen s použití HiLo strategie generování identifikátorů entit.
	/// </summary>
	ValueTask AddForInsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
		where TEntity : class;

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class;

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// Určeno pro použití s entitami využívajícími asynchronní operace, typicky jen s použití HiLo strategie generování identifikátorů entit.
	/// </summary>
	ValueTask AddRangeForInsertAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		where TEntity : class;

	/// <summary>
	/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
	/// </summary>
	void AddForUpdate<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
	/// </summary>
	void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class;

	/// <summary>
	/// Zajistí odstranění objektu (při uložení bude smazán).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	void AddForDelete<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Zajistí odstranění objektů (při uložení budou smazány).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class;

	/// <summary>
	/// Registruje akci k provedení po (synchronním i asynchronním) commitu.
	/// </summary>
	void RegisterAfterCommitAction(Action action);

	/// <summary>
	/// Registruje asynchronní akci k provedení po asynchronním commitu.
	/// Použití synchronního commitu po registraci asynchronní akce vyhodí výjimku.
	/// </summary>
	void RegisterAfterCommitAction(Func<CancellationToken, Task> asyncAction);

	/// <summary>
	/// Vyčistí registrace objektů, after commit actions, atp. (vč. podkladového DbContextu a jeho changetrackeru).
	/// </summary>
	void Clear();
}
