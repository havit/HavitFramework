using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínku testující logickou hodnotu.
	/// </summary>	
	public static class BoolCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoøí podmínku pro vlastnost rovnou dané hodnotì.
		/// Je-li hodnota value null, testuje se operand na null (IS NULL).
		/// </summary>
		public static Condition CreateEquals(IOperand operand, bool? value)
		{
			if (value == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			else
			{
				return new BinaryCondition(BinaryCondition.EqualsPattern, operand, ValueOperand.Create(value.Value));
			}
		} 

		/// <summary>
		/// Vytvoøí podmínku porovnávající hodnoty dvou operandù na rovnost.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, operand1, operand2);
		} 
		#endregion

		#region CreateNotEquals
		/// <summary>
		/// Vytvoøí podmínku porovnávající hodnoty dvou operandù na nerovnost.
		/// Hodnota null není žádným zpùsobem zpracovávána, tj. pokud alespoò jeden operand má hodnotu null, není ve výsledku dotazu.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(BinaryCondition.NotEqualsPattern, operand1, operand2);
		} 
		#endregion

		#region CreateTrue
		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu true.
		/// </summary>
		public static Condition CreateTrue(IOperand operand)
		{
			return CreateEquals(operand, true);
		} 
		#endregion

		#region CreateFalse
		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu false.
		/// </summary>
		public static Condition CreateFalse(IOperand operand)
		{
			return CreateEquals(operand, false);
		} 
		#endregion
	}
}
