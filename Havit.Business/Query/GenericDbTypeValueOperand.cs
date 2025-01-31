using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Havit.Data.SqlServer;

namespace Havit.Business.Query;

internal sealed class GenericDbTypeValueOperand : IOperand
{
	/// <summary>
	/// Hodnota konstanty ValueOperandu.
	/// </summary>
	private readonly object value;

	/// <summary>
	/// Databázový typ nesený ValueOperandem.
	/// </summary>
	private readonly DbType dbType;

	/// <summary>
	/// Vytvoří instanci třídy ValueOperand.
	/// </summary>
	public GenericDbTypeValueOperand(object value, DbType dbType)
	{
		this.value = value;
		this.dbType = dbType;
	}

	string IOperand.GetCommandValue(System.Data.Common.DbCommand command, SqlServerPlatform sqlServerPlatform)
	{
		Debug.Assert(command != null);

		DbParameter parameter = command.CreateParameter();
		parameter.ParameterName = ValueOperand.GetParameterName(command);
		parameter.Value = value ?? DBNull.Value;
		parameter.DbType = dbType;
		command.Parameters.Add(parameter);

		return parameter.ParameterName;
	}
}
