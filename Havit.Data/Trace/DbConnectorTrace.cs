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
	internal class DbConnectorTrace
	{
		#region Private fields
		private Stopwatch durationStopWatch;
		DbCommandTraceData traceData;
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		internal DbConnectorTrace(DbCommand dbCommand, string operation)
		{
			traceData = DbCommandTraceData.Create(dbCommand, operation);
			durationStopWatch = Stopwatch.StartNew();
		}
		#endregion

		#region SetResult
		/// <summary>
		/// Set command result.
		/// </summary>
		/// <param name="result"></param>
		public void SetResult(object result)
		{
			traceData.ResultSet = true;
			traceData.Result = result;
		}
		#endregion

		#region Trace
		/// <summary>
		/// Set DurationProperty and traces on TraceSource.
		/// </summary>
		internal void Trace(System.Diagnostics.TraceSource traceSource)
		{
			durationStopWatch.Stop();
			traceData.Duration = durationStopWatch.ElapsedMilliseconds;
			traceSource.TraceData(TraceEventType.Information, 0, traceData);
		}
		#endregion		

	}	
}
