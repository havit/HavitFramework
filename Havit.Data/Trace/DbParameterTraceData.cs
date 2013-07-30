using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Trace
{
	/// <summary>
	/// Trace data for DbParameter.
	/// </summary>
	public class DbParameterTraceData
	{
		#region ParameterName
		/// <summary>
		/// Parameter name.
		/// </summary>
		public string ParameterName { get; private set; }
		#endregion

		#region Direction
		/// <summary>
		/// Parameter direction.
		/// </summary>
		public ParameterDirection Direction { get; private set; }
		#endregion

		#region DbType
		/// <summary>
		/// Parameter type.
		/// </summary>
		public DbType DbType { get; private set; }
		#endregion

		#region Value
		/// <summary>
		/// Parameter value.
		/// </summary>
		public object Value { get; private set; }
		#endregion

		#region Create
		/// <summary>
		/// Creates an instance of DbParameterTraceData from DbParameter.
		/// </summary>
		internal static DbParameterTraceData Create(DbParameter dbParameter)
		{
			DbParameterTraceData result = new DbParameterTraceData();

			result.ParameterName = dbParameter.ParameterName;
			result.DbType = dbParameter.DbType;
			result.Value = dbParameter.Value;
			result.Direction = dbParameter.Direction;

			return result;
		}
		#endregion
	}
}
