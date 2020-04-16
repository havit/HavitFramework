using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataEntries
{
	/// <summary>
	/// Párování enumů na identifikátor.
	/// </summary>
	/// <remarks>
	/// Generický typ je zde kvůli DI containeru - pro každý typ entity se udělá vlastní singleton.
	/// </remarks>
	public class DataEntrySymbolStorage<TEntity> : IDataEntrySymbolStorage<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Úložiště párování enumů na identifikátor.
		/// </summary>
		public Dictionary<string, int> Value { get; set; }

	}
}
