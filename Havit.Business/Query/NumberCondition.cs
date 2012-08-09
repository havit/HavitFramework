using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující èíselné hodnoty.
	/// </summary>
	public static class NumberCondition
	{
		/// <summary>
		/// Vytvoøí podmínku testující rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, int value)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, int value)
		{
			return new BinaryCondition(operand, BinaryCondition.GetComparisonPattern(comparisonOperator), ValueOperand.Create(value));			
		}

	}
}
