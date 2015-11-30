using System;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataEntries
{
	// TODO: To Havit.Patterns?

	/// <summary>
	/// Předek pro implementace IDataEntries pro jednotlivé entity.
	/// </summary>
	/// <typeparam name="TEntity">The type of the t entity.</typeparam>
	public abstract class DbDataEntries<TEntity>
		where TEntity : class
	{
		private readonly IDataEntrySymbolStorage<TEntity> dataEntrySymbolStorage;
		private readonly IRepository<TEntity> repository;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="dataEntrySymbolStorage">Úložiště mapování párovacích symbolů a identifikátorů objektů.</param>
		/// <param name="repository">Repository pro získání objektu dle identifikátoru.</param>
		protected DbDataEntries(IDataEntrySymbolStorage<TEntity> dataEntrySymbolStorage, IRepository<TEntity> repository)
		{
			Contract.Requires(dataEntrySymbolStorage != null);
			Contract.Requires(repository != null);

			this.dataEntrySymbolStorage = dataEntrySymbolStorage;
			this.repository = repository;
		}

		/// <summary>
		/// Vrátí objekt pro daný "symbol" (hodnota enum).
		/// </summary>
		protected internal TEntity GetEntry(Enum entry)
		{
			// najdeme identifikátor objektu
			int id = dataEntrySymbolStorage.GetEntryId(entry);
			// vrátíme objekt z repository
			return repository.GetObject(id);
		}
	}
}