using System;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataEntries
{
	/// <summary>
	/// Předek pro implementace IDataEntries pro jednotlivé entity.
	/// </summary>
	/// <typeparam name="TEntity">The type of the t entity.</typeparam>
	public abstract class DataEntries<TEntity>
		where TEntity : class
	{
		private readonly IDataEntrySymbolStorage<TEntity> dataEntrySymbolStorage;
		private readonly IRepository<TEntity> repository;

		/// <summary>
		/// Konstruktor.
		/// Hodnota enumu je přímo mapována na identifikátor.
		/// </summary>
		/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
		protected DataEntries(IRepository<TEntity> repository)
		{
			Contract.Requires(repository != null);

			this.dataEntrySymbolStorage = null;
			this.repository = repository;
		}

		/// <summary>
		/// Konstruktor.
		/// Hodnota enumu je na identifikátor mapována pomocí dataEntrySymbolStorage (hodnota se hledá na základě symbolu).
		/// </summary>
		/// <param name="dataEntrySymbolStorage">Úložiště mapování párovacích symbolů a identifikátorů objektů.</param>
		/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
		protected DataEntries(IDataEntrySymbolStorage<TEntity> dataEntrySymbolStorage, IRepository<TEntity> repository)
		{
			Contract.Requires(repository != null);

			this.dataEntrySymbolStorage = dataEntrySymbolStorage;
			this.repository = repository;
		}

		/// <summary>
		/// Vrátí objekt pro daný enum.
		/// Pokud byla v konstruktoru předá dataEntrySymbolStorage, je mapování provedeno přes ni (mapování přes "symbol"),
		/// pokud nebyla předána, pak dojde k přímému mapování enumu na int.
		/// </summary>
		protected internal TEntity GetEntry(Enum entry)
		{
			// najdeme identifikátor objektu
			int id = (dataEntrySymbolStorage == null)
				? Convert.ToInt32(entry) // pokud hodnota enumu odpovídá identifikátoru, vezmeme ji přímo
				: dataEntrySymbolStorage.GetEntryId(entry); // pokud hodnota enum nemusí odpovídat identifikátoru, pak jej hledáme ve slovníku

			// vrátíme objekt z repository
			return repository.GetObject(id);
		}
	}
}