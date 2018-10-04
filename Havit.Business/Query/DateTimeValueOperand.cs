using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.SqlServer;

namespace Havit.Business.Query
{
	internal sealed class DateTimeValueOperand : IOperand
	{
		#region Private fields
		private readonly DateTime value;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci třídy DateTimeValueOperand.
		/// </summary>
		public DateTimeValueOperand(DateTime value)
		{
			this.value = value;
		}
		#endregion

		#region IOperand Members
		string IOperand.GetCommandValue(System.Data.Common.DbCommand command, SqlServerPlatform sqlServerPlatform)
		{
			Debug.Assert(command != null);

			DbParameter parameter = command.CreateParameter();
			parameter.ParameterName = ValueOperand.GetParameterName(command);
			parameter.Value = value;
			parameter.DbType = sqlServerPlatform >= SqlServerPlatform.SqlServer2008 ? DbType.DateTime2 : DbType.DateTime;
			command.Parameters.Add(parameter);

			return parameter.ParameterName;
		}
		#endregion
	}
}
