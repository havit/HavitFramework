using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business.Query
{
	/// <summary>
	/// Interface pro operandy SQL dotazu.
	/// Operandem mùže být výraz, databázový sloupec, sklalární hodnota...
	/// </summary>
	public interface IOperand
	{
		/// <summary>
		/// Vrací øetìzec, který reprezentuje hodnotu operandu v SQL dotazu.
		/// Mùže pøidávat databázové parametry do commandu.
		/// </summary>
		/// <param name="command">Databázový pøíkaz. Je možné do nìj pøidávat databázové parametry.</param>
		/// <returns>Øetìzec reprezentující hodnotu operandu v SQL dotazu.</returns>
		string GetCommandValue(DbCommand command);
	}
}
