using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Havit.Business.Query
{
	/// <summary>
	/// Konstanta jako operand databázového dotazu.
	/// </summary>
	[Serializable]
	public sealed class ValueOperand : IOperand
	{	
		#region Private fields
		/// <summary>
		/// Hodnota konstanty ValueOperandu.
		/// </summary>
		private object value;

		/// <summary>
		/// Databázový typ nesený ValueOperandem.
		/// </summary>
		private DbType dbType;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci třídy ValueOperand.
		/// </summary>
		public ValueOperand(object value, DbType dbType)
		{
			this.value = value;
			this.dbType = dbType;
		}
		#endregion

		#region IOperand Members
		string IOperand.GetCommandValue(System.Data.Common.DbCommand command)
		{
			string parameterName;
			int index = 1;
			do
			{
				parameterName = "@param" + (command.Parameters.Count + index).ToString();
				index += 1;
			}
			while (command.Parameters.Contains(parameterName));

			DbParameter parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.Value = value ?? DBNull.Value;
			parameter.DbType = dbType;
			command.Parameters.Add(parameter);

			return parameterName;
		}

		#endregion

		#region Create - Boolean
		/// <summary>
		/// Vytvoří operand z logické hodnoty.
		/// </summary>
		public static IOperand Create(bool value)
		{
			return new ValueOperand(value, DbType.Boolean);
		}
		#endregion

		#region Create - DateTime
		/// <summary>
		/// Vytvoří operand z DateTime.
		/// </summary>
		public static IOperand Create(DateTime value)
		{
			return new ValueOperand(value, DbType.DateTime);
		}
		#endregion

		#region Create - Int16, Int32, Int64
		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int16 value)
		{
			return new ValueOperand(value, DbType.Int16);
		}

		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int32 value)
		{
			return new ValueOperand(value, DbType.Int32);
		}

		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int64 value)
		{
			return new ValueOperand(value, DbType.Int64);
		}
		#endregion

		#region Create - Single, Double, Decimal
		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(Single value)
		{
			return new ValueOperand(value, DbType.Single);
		}

		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(Double value)
		{
			return new ValueOperand(value, DbType.Double);
		}

		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(decimal value)
		{
			return new ValueOperand(value, DbType.Decimal);
		}
		#endregion

		#region Create - GUID
		/// <summary>
		/// Vytvoří operand z GUIDu.
		/// </summary>
		public static IOperand Create(Guid value)
		{
			return new ValueOperand(value, DbType.Guid);
		}
		#endregion

		#region Create - String
		/// <summary>
		/// Vytvoří operand z řetězce.
		/// </summary>
		public static IOperand Create(string value)
		{
			return new ValueOperand(value, DbType.String);
		}
		#endregion
	}
}
