using System;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataEntries;

/// <summary>
/// Předek pro implementace IDataEntries pro jednotlivé entity.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
public abstract class DataEntries<TEntity>
	where TEntity : class
{
	private readonly IDataEntrySymbolService<TEntity> dataEntrySymbolService;
	private readonly IRepository<TEntity> repository; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbRepository.

	/// <summary>
	/// Konstruktor.
	/// Hodnota enumu je přímo mapována na identifikátor.
	/// </summary>
	/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
	protected DataEntries(IRepository<TEntity> repository)
	{
		Contract.Requires(repository != null);

		this.dataEntrySymbolService = null;
		this.repository = repository;
	}

	/// <summary>
	/// Konstruktor.
	/// Hodnota enumu je na identifikátor mapována pomocí dataEntrySymbolService (hodnota se hledá na základě symbolu).
	/// </summary>
	/// <param name="dataEntrySymbolService">Úložiště mapování párovacích symbolů a identifikátorů objektů.</param>
	/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
	protected DataEntries(IDataEntrySymbolService<TEntity> dataEntrySymbolService, IRepository<TEntity> repository)
	{
		Contract.Requires(repository != null);

		this.dataEntrySymbolService = dataEntrySymbolService;
		this.repository = repository;
	}

	/// <summary>
	/// Vrátí objekt pro daný enum.
	/// Pokud byla v konstruktoru předá dataEntrySymbolService, je mapování provedeno přes ni (mapování přes "symbol"),
	/// pokud nebyla předána, pak dojde k přímému mapování enumu na int.
	/// </summary>
	protected internal TEntity GetEntry(Enum entry)
	{
		// najdeme identifikátor objektu
		int id = (dataEntrySymbolService == null)
			? Convert.ToInt32(entry) // pokud hodnota enumu odpovídá identifikátoru, vezmeme ji přímo
			: dataEntrySymbolService.GetEntryId(entry); // pokud hodnota enum nemusí odpovídat identifikátoru, pak jej hledáme ve slovníku

		// vrátíme objekt z repository
		return repository.GetObject(id);
	}
}