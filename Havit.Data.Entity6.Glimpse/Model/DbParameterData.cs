using System.Data;
using System.Data.Common;

namespace Havit.Data.Entity.Glimpse.Model
{
	/// <summary>
	/// DbParameter values.
	/// </summary>
	public class DbParameterData
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
		internal static DbParameterData Create(DbParameter dbParameter)
		{
			DbParameterData result = new DbParameterData();

			result.ParameterName = dbParameter.ParameterName;
			result.DbType = dbParameter.DbType;
			result.Value = dbParameter.Value;
			result.Direction = dbParameter.Direction;

			return result;
		}
		#endregion
	}
}
