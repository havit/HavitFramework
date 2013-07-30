using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Trace
{
	/// <summary>
	/// Trace constants.
	/// </summary>
	public static class Consts
	{
		#region CommandExecutionTraceSourceName
		/// <summary>
		/// Trace source name for tracing DbConnector command executions.
		/// </summary>
		public const string CommandExecutionTraceSourceName = "DbConnector Command Execution Trace";
		#endregion

		#region CommandExecutedID
		/// <summary>
		/// Trace source command ID for tracing DbConnector command executions.
		/// </summary>
		public const int CommandExecutedID = 0;
		#endregion
	}
}
