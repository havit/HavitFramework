using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Glimpse.DbCommandInterception
{
	/// <summary>
	/// Dekorátor k DbDataReaderu, který počítá počet záznamů.
	/// </summary>
	internal class RecordCountingDbDataReader : DbDataReader
	{
		#region Private fields
		private readonly DbDataReader wrappedDataReader;
		private readonly DbDataReaderResult dbDataReaderResult;
		private int recordsCount;
		#endregion

		#region Depth
		public override int Depth
		{
			get
			{
				return wrappedDataReader.Depth;
			}
		}
		#endregion

		#region FieldCount
		public override int FieldCount
		{
			get
			{
				return wrappedDataReader.FieldCount;
			}
		}
		#endregion

		#region HasRows
		public override bool HasRows
		{
			get
			{
				return wrappedDataReader.HasRows;
			}
		}
		#endregion

		#region IsClosed
		public override bool IsClosed
		{
			get
			{
				return wrappedDataReader.IsClosed;
			}
		}
		#endregion

		#region RecordsAffected
		public override int RecordsAffected
		{
			get
			{
				return wrappedDataReader.RecordsAffected;
			}
		}
		#endregion

		#region Constructor
		public RecordCountingDbDataReader(DbDataReader dataReader, DbDataReaderResult dbDataReaderResult)
		{			
			this.wrappedDataReader = dataReader;
			this.dbDataReaderResult = dbDataReaderResult;
			this.recordsCount = 0;
		}
		#endregion

		#region Close
		public override void Close()
		{
			while (this.Read()) /* spočítáme nepřečtené záznamy */
			{
				// NOOP;
			}
			wrappedDataReader.Close();

			dbDataReaderResult.RecordsCount = recordsCount;
		}
		#endregion

		#region GetBoolean
		public override bool GetBoolean(int ordinal)
		{
			return wrappedDataReader.GetBoolean(ordinal);
		}
		#endregion

		#region GetByte
		public override byte GetByte(int ordinal)
		{
			return wrappedDataReader.GetByte(ordinal);
		}
		#endregion

		#region GetBytes
		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			return wrappedDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
		}
		#endregion

		#region GetChar
		public override char GetChar(int ordinal)
		{
			return wrappedDataReader.GetChar(ordinal);
		}
		#endregion

		#region GetChars
		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			return wrappedDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
		}
		#endregion

		#region GetDataTypeName
		public override string GetDataTypeName(int ordinal)
		{
			return wrappedDataReader.GetDataTypeName(ordinal);
		}
		#endregion

		#region GetDateTime
		public override DateTime GetDateTime(int ordinal)
		{
			return wrappedDataReader.GetDateTime(ordinal);
		}
		#endregion

		#region GetDecimal
		public override decimal GetDecimal(int ordinal)
		{
			return wrappedDataReader.GetDecimal(ordinal);
		}
		#endregion

		#region GetDouble
		public override double GetDouble(int ordinal)
		{
			return wrappedDataReader.GetDouble(ordinal);
		}
		#endregion

		#region GetEnumerator
		public override System.Collections.IEnumerator GetEnumerator()
		{
			return wrappedDataReader.GetEnumerator();
		}
		#endregion

		#region GetFieldType
		public override Type GetFieldType(int ordinal)
		{
			return wrappedDataReader.GetFieldType(ordinal);
		}
		#endregion

		#region GetFloat
		public override float GetFloat(int ordinal)
		{
			return wrappedDataReader.GetFloat(ordinal);
		}
		#endregion

		#region GetGuid
		public override Guid GetGuid(int ordinal)
		{
			return wrappedDataReader.GetGuid(ordinal);
		}
		#endregion

		#region GetInt16
		public override short GetInt16(int ordinal)
		{
			return wrappedDataReader.GetInt16(ordinal);
		}
		#endregion

		#region GetInt32
		public override int GetInt32(int ordinal)
		{
			return wrappedDataReader.GetInt32(ordinal);
		}
		#endregion

		#region GetInt64
		public override long GetInt64(int ordinal)
		{
			return wrappedDataReader.GetInt64(ordinal);
		}
		#endregion

		#region GetName
		public override string GetName(int ordinal)
		{
			return wrappedDataReader.GetName(ordinal);
		}
		#endregion

		#region GetOrdinal
		public override int GetOrdinal(string name)
		{
			return wrappedDataReader.GetOrdinal(name);
		}
		#endregion

		#region GetSchemaTable
		public override System.Data.DataTable GetSchemaTable()
		{
			return wrappedDataReader.GetSchemaTable();
		}
		#endregion

		#region GetString
		public override string GetString(int ordinal)
		{
			return wrappedDataReader.GetString(ordinal);
		}
		#endregion

		#region GetValue
		public override object GetValue(int ordinal)
		{
			return wrappedDataReader.GetValue(ordinal);
		}
		#endregion

		#region GetValues
		public override int GetValues(object[] values)
		{
			return wrappedDataReader.GetValues(values);
		}
		#endregion

		#region IsDBNull
		public override bool IsDBNull(int ordinal)
		{
			return wrappedDataReader.IsDBNull(ordinal);
		}
		#endregion

		#region NextResult
		public override bool NextResult()
		{
			return wrappedDataReader.NextResult();
		}
		#endregion

		#region Read
		public override bool Read()
		{
			bool result = wrappedDataReader.Read();
			if (result)
			{
				recordsCount += 1;
			}
			return result;
		}
		#endregion

		#region Indexers
		public override object this[string name]
		{
			get
			{
				return wrappedDataReader[name];
			}
		}

		public override object this[int ordinal]
		{
			get
			{
				return wrappedDataReader[ordinal];
			}
		}
		#endregion

		#region Dispose
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		#endregion
	}
}
