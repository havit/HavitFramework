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
	public class ValueOperand: IOperand
	{
	
		#region Private fields		
		/// <summary>
		/// Hodnota konstanty ValueOperandu.
		/// </summary>
		object value;

		/// <summary>
		/// Databázový typ nesený ValueOperandem.
		/// </summary>
		DbType dbType;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci tøídy ValueOperand.
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

		#region Create - Boolean
		/// <summary>
		/// Vytvoøí operand z logické hodnoty.
		/// </summary>
		public static IOperand Create(bool value)
		{
			return new ValueOperand(value, DbType.Boolean);
		}
		#endregion

		#region Create - DateTime
		/// <summary>
		/// Vytvoøí operand z DateTime.
		/// </summary>
		public static IOperand Create(DateTime value)
		{
			return new ValueOperand(value, DbType.DateTime);
		}
		#endregion

		#region Create - Int16, Int32, Int64
		/// <summary>
		/// Vytvoøí operand z celého èísla.
		/// </summary>
		public static IOperand Create(Int16 value)
		{
			return new ValueOperand(value, DbType.Int16);
		}

		/// <summary>
		/// Vytvoøí operand z celého èísla.
		/// </summary>
		public static IOperand Create(Int32 value)
		{
			return new ValueOperand(value, DbType.Int32);
		}

		/// <summary>
		/// Vytvoøí operand z celého èísla.
		/// </summary>
		public static IOperand Create(Int64 value)
		{
			return new ValueOperand(value, DbType.Int64);
		}
		#endregion

		#region Create - Single, Double, Decimal
		/// <summary>
		/// Vytvoøí operand z èísla.
		/// </summary>
		public static IOperand Create(Single value)
		{
			return new ValueOperand(value, DbType.Single);
		}

		/// <summary>
		/// Vytvoøí operand z èísla.
		/// </summary>
		public static IOperand Create(Double value)
		{
			return new ValueOperand(value, DbType.Double);
		}

		/// <summary>
		/// Vytvoøí operand z èísla.
		/// </summary>
		public static IOperand Create(decimal value)
		{
			return new ValueOperand(value, DbType.Decimal);
		}
		#endregion

		#region Create - GUID
		/// <summary>
		/// Vytvoøí operand z GUIDu.
		/// </summary>
		public static IOperand Create(Guid value)
		{
			return new ValueOperand(value, DbType.Guid);
		}
		#endregion

		#region Create - String
		/// <summary>
		/// Vytvoøí operand z øetìzce.
		/// </summary>
		public static IOperand Create(string value)
		{
			return new ValueOperand(value, DbType.String);
		}
		#endregion
	}
}
