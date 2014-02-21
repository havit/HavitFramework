using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Havit.Data.SqlServer;

namespace Havit.Business.Query
{
	/// <summary>
	/// Konstanta jako operand databázového dotazu 
	/// </summary>
	public static class ValueOperand
	{	
		#region Create - Boolean
		/// <summary>
		/// Vytvoří operand z logické hodnoty.
		/// </summary>
		public static IOperand Create(bool value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Boolean);
		}
		#endregion

		#region Create - DateTime
		/// <summary>
		/// Vytvoří operand z DateTime.
		/// </summary>
		public static IOperand Create(DateTime value)
		{
			return new DateTimeValueOperand(value);
		}
		#endregion

		#region Create - Int16, Int32, Int64
		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int16 value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Int16);
		}

		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int32 value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Int32);
		}

		/// <summary>
		/// Vytvoří operand z celého čísla.
		/// </summary>
		public static IOperand Create(Int64 value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Int64);
		}
		#endregion

		#region Create - Single, Double, Decimal
		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(Single value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Single);
		}

		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(Double value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Double);
		}

		/// <summary>
		/// Vytvoří operand z čísla.
		/// </summary>
		public static IOperand Create(decimal value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Decimal);
		}
		#endregion

		#region Create - GUID
		/// <summary>
		/// Vytvoří operand z GUIDu.
		/// </summary>
		public static IOperand Create(Guid value)
		{
			return new GenericDbTypeValueOperand(value, DbType.Guid);
		}
		#endregion

		#region Create - String
		/// <summary>
		/// Vytvoří operand z řetězce.
		/// </summary>
		public static IOperand Create(string value)
		{
			return new GenericDbTypeValueOperand(value, DbType.String);
		}
		#endregion

		#region GetParameterName
		/// <summary>
		/// Vrátí jméno pro nový parametr.
		/// </summary>
		internal static string GetParameterName(DbCommand command)
		{
			string parameterName;
			int index = 1;
			do
			{
				parameterName = "@param" + (command.Parameters.Count + index).ToString();
				index += 1;
			}
			while (command.Parameters.Contains(parameterName));

			return parameterName;
		}
		#endregion
	}
}
