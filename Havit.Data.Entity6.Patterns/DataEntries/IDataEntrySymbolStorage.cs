using System;
using Havit.Data.Patterns.Exceptions;

namespace Havit.Data.Entity.Patterns.DataEntries
{
	/// <summary>
	/// Zajišťuje mapování párovacích symbolů a identifikátorů objektů, resp. získání identifikátoru (primárního klíče) na základě symbolu.
	/// </summary>
	public interface IDataEntrySymbolStorage<TEntity>
	{
		/// <summary>
		/// Vrací hodnotu identifikátoru (primárního klíče) na základě symbolu.		
		/// </summary>
		/// <param name="entry">"Symbol".</param>
		/// <exception cref="ObjectNotFoundException">Pokud není identifikátor dle symbolu nalezen.</exception>
		int GetEntryId(Enum entry);
	}
}
