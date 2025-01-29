using Havit.Data.Patterns.Repositories;

namespace Havit.Data.Patterns.DataEntries;

/// <summary>
/// Předek pro implementace IDataEntries pro jednotlivé entity.
/// </summary>
/// <remarks>
/// Určeno pro Entity Framework 6, který používá <see cref="IRepository{TEntity}"/>, ale nikoliv <see cref="IRepository{TEntity, TKey}"/>.
/// </remarks>
public abstract class DataEntries<TEntity> : DataEntries<TEntity, int>
	where TEntity : class
{
	/// <summary>
	/// Konstruktor.
	/// Hodnota enumu je přímo mapována na identifikátor.
	/// </summary>
	/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
	protected DataEntries(IRepository<TEntity, int> repository) : base(repository)
	{
	}

	/// <summary>
	/// Konstruktor.
	/// Hodnota enumu je na identifikátor mapována pomocí dataEntrySymbolService (hodnota se hledá na základě symbolu).
	/// </summary>
	/// <param name="dataEntrySymbolService">Úložiště mapování párovacích symbolů a identifikátorů objektů.</param>
	/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
	protected DataEntries(IDataEntrySymbolService<TEntity, int> dataEntrySymbolService, IRepository<TEntity, int> repository)
		: base(dataEntrySymbolService, repository)
	{
	}

}