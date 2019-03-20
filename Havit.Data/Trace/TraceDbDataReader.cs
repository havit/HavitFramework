using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Trace
{
	internal class TraceDbDataReader : DbDataReader
	{
		private readonly DbDataReader wrappedDataReader;
		private readonly DbConnectorTrace dbConnectorTrace;
		private int recordsCount;

		public override int Depth
		{
			get
			{
				return wrappedDataReader.Depth;
			}
		}

		public override int FieldCount
		{
			get
			{
				return wrappedDataReader.FieldCount;
			}
		}

		public override bool HasRows
		{
			get
			{
				return wrappedDataReader.HasRows;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return wrappedDataReader.IsClosed;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return wrappedDataReader.RecordsAffected;
			}
		}

		public TraceDbDataReader(DbDataReader dataReader, DbConnectorTrace dbConnectorTrace)
		{
			this.wrappedDataReader = dataReader;
			this.dbConnectorTrace = dbConnectorTrace;
			this.recordsCount = 0;
		}

		public override void Close()
		{
			while (this.Read()) /* spočítáme nepřečtené záznamy */
			{
				// NOOP;
			}

			dbConnectorTrace.SetResult(String.Format("{0} record{1}",
				this.recordsCount,
				(this.recordsCount == 1) ? "" : "s"));

			wrappedDataReader.Close();
		}

		public override bool GetBoolean(int ordinal)
		{
			return wrappedDataReader.GetBoolean(ordinal);
		}

		public override byte GetByte(int ordinal)
		{
			return wrappedDataReader.GetByte(ordinal);
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			return wrappedDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
		}

		public override char GetChar(int ordinal)
		{
			return wrappedDataReader.GetChar(ordinal);
		}

		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			return wrappedDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
		}

		public override string GetDataTypeName(int ordinal)
		{
			return wrappedDataReader.GetDataTypeName(ordinal);
		}

		public override DateTime GetDateTime(int ordinal)
		{
			return wrappedDataReader.GetDateTime(ordinal);
		}

		public override decimal GetDecimal(int ordinal)
		{
			return wrappedDataReader.GetDecimal(ordinal);
		}

		public override double GetDouble(int ordinal)
		{
			return wrappedDataReader.GetDouble(ordinal);
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return wrappedDataReader.GetEnumerator();
		}

		public override Type GetFieldType(int ordinal)
		{
			return wrappedDataReader.GetFieldType(ordinal);
		}

		public override float GetFloat(int ordinal)
		{
			return wrappedDataReader.GetFloat(ordinal);
		}

		public override Guid GetGuid(int ordinal)
		{
			return wrappedDataReader.GetGuid(ordinal);
		}

		public override short GetInt16(int ordinal)
		{
			return wrappedDataReader.GetInt16(ordinal);
		}

		public override int GetInt32(int ordinal)
		{
			return wrappedDataReader.GetInt32(ordinal);
		}

		public override long GetInt64(int ordinal)
		{
			return wrappedDataReader.GetInt64(ordinal);
		}

		public override string GetName(int ordinal)
		{
			return wrappedDataReader.GetName(ordinal);
		}

		public override int GetOrdinal(string name)
		{
			return wrappedDataReader.GetOrdinal(name);
		}

		public override System.Data.DataTable GetSchemaTable()
		{
			return wrappedDataReader.GetSchemaTable();
		}

		public override string GetString(int ordinal)
		{
			return wrappedDataReader.GetString(ordinal);
		}

		public override object GetValue(int ordinal)
		{
			return wrappedDataReader.GetValue(ordinal);
		}

		public override int GetValues(object[] values)
		{
			return wrappedDataReader.GetValues(values);
		}

		public override bool IsDBNull(int ordinal)
		{
			return wrappedDataReader.IsDBNull(ordinal);
		}

		public override bool NextResult()
		{
			return wrappedDataReader.NextResult();
		}

		public override bool Read()
		{
			bool result = wrappedDataReader.Read();
			if (result)
			{
				recordsCount += 1;
			}
			return result;
		}

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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
