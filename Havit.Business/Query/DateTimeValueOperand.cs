using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.SqlServer;

namespace Havit.Business.Query;

internal sealed class DateTimeValueOperand : IOperand
{
	private readonly DateTime value;

	/// <summary>
	/// Vytvoří instanci třídy DateTimeValueOperand.
	/// </summary>
	public DateTimeValueOperand(DateTime value)
	{
		this.value = value;
	}

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
}
