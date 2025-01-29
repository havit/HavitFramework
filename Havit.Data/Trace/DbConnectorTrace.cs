﻿using System.Data.Common;
using System.Diagnostics;

namespace Havit.Data.Trace;

/// <summary>
/// Trace data for DbCommand.
/// </summary>
internal class DbConnectorTrace
{
	private readonly Stopwatch durationStopWatch;
	private readonly DbCommandTraceData traceData;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	internal DbConnectorTrace(DbCommand dbCommand, string operation)
	{
		traceData = DbCommandTraceData.Create(dbCommand, operation);
		durationStopWatch = Stopwatch.StartNew();
	}

	/// <summary>
	/// Set command result.
	/// </summary>
	public void SetResult(object result)
	{
		traceData.ResultSet = true;
		traceData.Result = result;
	}

	/// <summary>
	/// Set DurationProperty and traces on TraceSource.
	/// </summary>
	internal void Trace(System.Diagnostics.TraceSource traceSource)
	{
		durationStopWatch.Stop();
		traceData.DurationTicks = durationStopWatch.Elapsed.Ticks;
		traceSource.TraceData(TraceEventType.Information, 0, traceData);
	}
}
