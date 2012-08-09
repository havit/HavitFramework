using System;
using System.Collections.Generic;
using System.Text;


namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující datumy.
	/// </summary>
	public static class DateCondition
	{
		/// <summary>
		/// Vytvoøí podmínku testující rovnost datumù.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, DateTime dateTime)
		{
			return new BinaryCondition(operand, BinaryCondition.EqualsPattern, ValueOperand.Create(dateTime));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, DateTime value)
		{
			return new BinaryCondition(operand, BinaryCondition.GetComparisonPattern(comparisonOperator), ValueOperand.Create(value));
		}

	}
}
