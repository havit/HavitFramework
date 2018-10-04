using Glimpse.Core.Extensibility;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Glimpse.SerializationConverters
{
   /// <summary>
    /// The <see cref="ISerializationConverter"/> implementation responsible converting SqlDataRecord[] representation's into strings.
    /// </summary>
 	public class SqlDataRecordArrayConverter : ISerializationConverter
	{
		#region SupportedTypes
		/// <summary>
		/// Gets the supported types the converter will be invoked for.
		/// </summary>
		/// <value>
		/// The supported type is SqlDataRecord[] (array of SqlDataRecord).
		/// </value>
		public IEnumerable<Type> SupportedTypes
		{
			get
			{
				yield return typeof(SqlDataRecord[]);
			}
		}
		#endregion

		#region Convert
		/// <summary>
		/// Converts the specified SqlDataRecord[] into string.
		/// </summary>
		public object Convert(object data)
		{
			SqlDataRecord[] sqlDataRecords = data as SqlDataRecord[];

			if (sqlDataRecords != null)
			{
				if ((sqlDataRecords.Length > 0) && (sqlDataRecords[0].FieldCount == 1))
				{
					return String.Join(", ", sqlDataRecords.Select(sqlDataRecord => sqlDataRecord.GetValue(0).ToString()));
				}
				else
				{
					return sqlDataRecords.ToString();
				}
			}
			else
			{
				return null;
			}
		}
		#endregion
    }
}
