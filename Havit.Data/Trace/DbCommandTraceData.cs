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
		#region Private fields
		private Stopwatch durationStopWatch;
		#endregion

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

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		private DbCommandTraceData()
		{
			Parameters = new List<DbParameterTraceData>();
			durationStopWatch = Stopwatch.StartNew();
		}
		#endregion

		#region Trace
		/// <summary>
		/// Set DurationProperty and traces on TraceSource.
		/// </summary>
		internal void Trace(System.Diagnostics.TraceSource traceSource)
		{
			this.Duration = durationStopWatch.ElapsedMilliseconds;
			traceSource.TraceData(TraceEventType.Information, Consts.CommandExecutedID, this);
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
			result.Parameters.AddRange(dbCommand.Parameters.Cast<DbParameter>().Select(dbParameter => DbParameterTraceData.Create(dbParameter)));
			return result;
		}
		#endregion

	}	
}
