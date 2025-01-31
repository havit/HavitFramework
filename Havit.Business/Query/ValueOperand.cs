using System.Data;
using System.Data.Common;

namespace Havit.Business.Query;

/// <summary>
/// Konstanta jako operand databázového dotazu 
/// </summary>
public static class ValueOperand
{
	/// <summary>
	/// Vytvoří operand z logické hodnoty.
	/// </summary>
	public static IOperand Create(bool value)
	{
		return new GenericDbTypeValueOperand(value, DbType.Boolean);
	}

	/// <summary>
	/// Vytvoří operand z DateTime.
	/// </summary>
	public static IOperand Create(DateTime value)
	{
		return new DateTimeValueOperand(value);
	}

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

	/// <summary>
	/// Vytvoří operand z GUIDu.
	/// </summary>
	public static IOperand Create(Guid value)
	{
		return new GenericDbTypeValueOperand(value, DbType.Guid);
	}

	/// <summary>
	/// Vytvoří operand z řetězce.
	/// </summary>
	public static IOperand Create(string value)
	{
		return new GenericDbTypeValueOperand(value, DbType.String);
	}

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
}
