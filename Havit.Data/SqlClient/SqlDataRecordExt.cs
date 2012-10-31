using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Server;

namespace Havit.Data.SqlClient
{
	/// <summary>
	/// Pomocné metody pro práci s SqlDataRecord.
	/// </summary>
	public static class SqlDataRecordExt
	{
		#region CreateForIntArrayTableType
		/// <summary>
		/// Vytvoří pole SqlDataRecors pro dané pole integerů.
		/// Pokud je ids null, vrací také null.
		/// </summary>
		/// <param name="ids">Parametry, pro každý je vytvořen jeden SqlDataRecord.</param>
		/// <returns></returns>
		public static SqlDataRecord[] CreateForIntArrayTableType(int[] ids)
		{
			if (ids == null)
			{
				return null;
			}

			int arraySize = ids.Length;
			SqlMetaData[] sqlMetaData = new SqlMetaData[] { new SqlMetaData("Value", SqlDbType.Int) };
			SqlDataRecord[] result = new SqlDataRecord[arraySize];
			for (int i = 0; i < arraySize; i++)
			{
				SqlDataRecord item = new SqlDataRecord(sqlMetaData);
				item.SetSqlInt32(0, ids[i]);
				result[i] = item;
			}
			return result;
		}
		#endregion
	}
}
