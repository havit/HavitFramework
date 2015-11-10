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

		#region DurationTicks
		/// <summary>
		/// Command execution duration (ticks).
		/// </summary>
		public long DurationTicks { get; internal set; }
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

		#region ToString
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(base.ToString());
			sb.AppendLine($"  Operation: {this.Operation}");
			sb.AppendLine($"  Command Text: {this.CommandText}");
				
			if (this.Parameters.Count > 0)
			{
				sb.AppendLine("  Parameters:");
				foreach (var parameter in Parameters)
				{					
					sb.AppendLine($"    {parameter.ParameterName}: {parameter.Value} (DbType.{parameter.DbType}, {parameter.Direction})");
				}
			}
			decimal durationMs = this.DurationTicks / (decimal)TimeSpan.TicksPerMillisecond;
            sb.AppendLine($"  Duration: {durationMs:N2} ms");
			
			if (ResultSet)
			{
				sb.AppendLine($"  Result: {this.Result ?? "null"} ");
			}

			return sb.ToString();
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
