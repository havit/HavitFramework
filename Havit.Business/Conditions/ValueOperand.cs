using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business.Conditions;
using System.Data;
using System.Data.Common;

namespace Havit.Business.Conditions
{
	public class ValueOperand: IOperand
	{
		object value;
		DbType dbType;

		#region IOperand Members

		string IOperand.GetCommandValue(System.Data.Common.DbCommand command)
		{
			string parameterName;
			do
			{
				parameterName = "@param" + new Random().Next(Int32.MaxValue);
			} while (command.Parameters.Contains(parameterName));

			DbParameter parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.Value = value;
			parameter.DbType = dbType;
			command.Parameters.Add(parameter);

			return parameterName;
		}

		#endregion

		public ValueOperand(object value, DbType dbType)
		{
			this.value = value;
			this.dbType = dbType;
		}

		public static IOperand FromString(string value)
		{
			return new ValueOperand(value, DbType.String);
		}

		public static IOperand FromInteger(int value)
		{
			return new ValueOperand(value, DbType.Int32);
		}

		public static IOperand FromBoolean(bool value)
		{
			return new ValueOperand(value, DbType.Boolean);
		}
	}
}
