﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

using Havit.Data.SqlTypes;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// IntTable jako operand databázového dotazu.
	/// </summary>
	public sealed class IntTableOperand : IOperand
	{
		#region Private fields
		/// <summary>
		/// Hodnota konstanty ValueOperandu.
		/// </summary>
		private int[] value;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci třídy IntTableOperand.
		/// </summary>
		private IntTableOperand(int[] value)
		{
			this.value = value;
		}
		#endregion

		#region Create
		/// <summary>
		/// Vytvoří operand z pole integerů.
		/// </summary>
		public static IOperand Create(int[] ids)
		{
			Contract.Requires<ArgumentNullException>(ids != null, "ids");
			
			return new IntTableOperand(ids);
		}
		#endregion

		#region IOperand Members
		string IOperand.GetCommandValue(System.Data.Common.DbCommand command)
		{
			Debug.Assert(command != null);

			if (!(command is SqlCommand))
			{
				throw new ArgumentException("Typ IntTableOperand předpokládá SqlCommand.");
			}

			SqlCommand sqlCommand = command as SqlCommand;

			string parameterName;
			int index = 1;
			do
			{
				parameterName = "@param" + (command.Parameters.Count + index).ToString();
				index += 1;
			}
			while (command.Parameters.Contains(parameterName));

			SqlParameter parameter = new SqlParameter();
			parameter.ParameterName = parameterName;
			parameter.SqlDbType = SqlDbType.Structured;
			parameter.TypeName = "dbo.IntTable";
			parameter.Value = IntTable.GetSqlParameterValue(this.value);
			sqlCommand.Parameters.Add(parameter);

			return parameterName;
		}
		#endregion
	}
}
