using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business.Query
{
	/// <summary>
	/// Interface pro operandy SQL dotazu.
	/// Operandem může být výraz, databázový sloupec, sklalární hodnota...
	/// </summary>
	public interface IOperand
	{
		#region GetCommandValue
		/// <summary>
		/// Vrací řetězec, který reprezentuje hodnotu operandu v SQL dotazu.
		/// Může přidávat databázové parametry do commandu.
		/// </summary>
		/// <param name="command">Databázový příkaz. Je možné do něj přidávat databázové parametry.</param>
		/// <returns>Řetězec reprezentující hodnotu operandu v SQL dotazu.</returns>
		string GetCommandValue(DbCommand command); 
		#endregion
	}
}
