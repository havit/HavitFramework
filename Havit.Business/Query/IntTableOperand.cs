using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query;

/// <summary>
/// IntTable jako operand databázového dotazu.
/// </summary>
public sealed class IntTableOperand : IOperand
{
	/// <summary>
	/// Hodnota konstanty ValueOperandu.
	/// </summary>
	private readonly int[] value;

	/// <summary>
	/// Vytvoří instanci třídy IntTableOperand.
	/// </summary>
	private IntTableOperand(int[] value)
	{
		this.value = value;
	}

	/// <summary>
	/// Vytvoří operand z pole integerů.
	/// </summary>
	public static IOperand Create(int[] ids)
	{
		Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));
		
		return new IntTableOperand(ids);
	}

	string IOperand.GetCommandValue(System.Data.Common.DbCommand command, SqlServerPlatform sqlServerPlatform)
	{
		Debug.Assert(command != null);
		Debug.Assert(sqlServerPlatform >= SqlServerPlatform.SqlServer2008);

		if (!(command is SqlCommand))
		{
			throw new ArgumentException("Typ IntTableOperand předpokládá SqlCommand.");
		}

		SqlCommand sqlCommand = command as SqlCommand;

		SqlParameter parameter = new SqlParameter();
		parameter.ParameterName = ValueOperand.GetParameterName(command);
		parameter.SqlDbType = SqlDbType.Structured;
		parameter.TypeName = "dbo.IntTable";
		parameter.Value = IntTable.GetSqlParameterValue(this.value);
		sqlCommand.Parameters.Add(parameter);

		return parameter.ParameterName;
	}
}
