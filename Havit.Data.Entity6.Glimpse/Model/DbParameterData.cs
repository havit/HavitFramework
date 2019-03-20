using System.Data;
using System.Data.Common;

namespace Havit.Data.Entity.Glimpse.Model
{
	/// <summary>
	/// DbParameter values.
	/// </summary>
	public class DbParameterData
	{
		/// <summary>
		/// Parameter name.
		/// </summary>
		public string ParameterName { get; private set; }

		/// <summary>
		/// Parameter direction.
		/// </summary>
		public ParameterDirection Direction { get; private set; }

		/// <summary>
		/// Parameter type.
		/// </summary>
		public DbType DbType { get; private set; }

		/// <summary>
		/// Parameter value.
		/// </summary>
		public object Value { get; private set; }

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
	}
}
