using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Server;

namespace Havit.Data.SqlTypes
{
	/// <summary>
	/// Pomocné metody pro práci s typem IntTable pro (table value parameter).
	/// </summary>
	public static class IntTable
	{
		/// <summary>
		/// Vytvoří hodnotu pro SqlParameter předávající dané pole integerů.
		/// Pokud je ids null nebo neobsahuje žádný záznam, vrací null.
		/// (Prázdné pole nelze předat (exception), takže buď se předává null nebo pole, které má alespoň jednu hodnotu.)
		/// Pokud obsahuje pole integerů duplicity, jsou odstraněny (každá hodnota se posílá jen jednou).
		/// </summary>		
		/// <param name="ids">Parametry, pro každý je vytvořen jeden SqlDataRecord.</param>
		public static object GetSqlParameterValue(int[] ids)
		{
			if ((ids == null) || (ids.Length == 0))
			{
				return null;
			}
			else
			{
				int[] distinctIDs = ids.Distinct().ToArray();
				int arraySize = distinctIDs.Length;
				SqlMetaData[] sqlMetaData = new SqlMetaData[] { new SqlMetaData("Value", SqlDbType.Int) };
				SqlDataRecord[] result = new SqlDataRecord[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					SqlDataRecord item = new SqlDataRecord(sqlMetaData);
					item.SetSqlInt32(0, distinctIDs[i]);
					result[i] = item;
				}
				return result;
			}
		}
	}
}
