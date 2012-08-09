using System;
using System.Data.Common;

namespace Havit.Business
{
	/// <summary>
	/// Interface pro Property popisující strukturu domény.	
	/// </summary>
	public interface IProperty
	{
		/// <summary>
		/// Vrátí řetězec pro vytažení daného sloupce z databáze.
		/// </summary>
		string GetSelectFieldStatement(DbCommand command);
	}
}
