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
		/// <summary>
		/// Vytvoøí podmínku pro vlastnost rovnou dané hodnotì.
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
		/// Vytvoøí podmínku testující vlastnost na hodnotu true.
		/// </summary>
		public static Condition CreateTrue(IOperand operand)
		{
			return CreateEquals(operand, true);
		}

		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu false.
		/// </summary>
		public static Condition CreateFalse(IOperand operand)
		{
			return CreateEquals(operand, false);
		}
	}
}
