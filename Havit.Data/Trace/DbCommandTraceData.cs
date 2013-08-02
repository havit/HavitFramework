using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Trace
{
	/// <summary>
	/// Trace data for DbCommand.
	/// </summary>
	public class DbCommandTraceData
	{
		#region Operation
		/// <summary>
		/// Command operation.
		/// </summary>
		public string Operation { get; private set; }
		#endregion

		#region CommandText
		/// <summary>
		/// Command text.
		/// </summary>
		public string CommandText { get; private set; }
		#endregion

		#region TransactionHashCode
		/// <summary>
		/// Transaction hash code.
		/// </summary>
		public int? TransactionHashCode { get; private set; }
		#endregion

		#region Parameters
		/// <summary>
		/// Command parameters.
		/// </summary>
		public List<DbParameterTraceData> Parameters { get; private set; }
		#endregion

		#region Duration
		/// <summary>
		/// Command execution duration.
		/// </summary>
		public long Duration { get; internal set; }
		#endregion

		#region ResultSet
		/// <summary>
		/// Command result set flag.
		/// </summary>
		public bool ResultSet { get; internal set; }
		#endregion

		#region Result
		/// <summary>
		/// Command result.
		/// </summary>
		public object Result { get; internal set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		private DbCommandTraceData()
		{
			Parameters = new List<DbParameterTraceData>();
		}
		#endregion

		#region Create
		/// <summary>
		/// Creates an instance of DbCommandTraceData from DbCommand.
		/// </summary>
		internal static DbCommandTraceData Create(DbCommand dbCommand, string operation)
		{
			DbCommandTraceData result = new DbCommandTraceData();
			result.Operation = operation;
			result.CommandText = dbCommand.CommandText;
			result.TransactionHashCode = (dbCommand.Transaction == null) ? (int?)null : dbCommand.Transaction.GetHashCode();
			result.Parameters.AddRange(dbCommand.Parameters.Cast<DbParameter>().Select(dbParameter => DbParameterTraceData.Create(dbParameter)));
			result.ResultSet = false;
			return result;
		}
		#endregion

	}	
}
