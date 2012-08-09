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
			return CreateEquals(operand, ValueOperand.Create(dateTime));			
		}

		/// <summary>
		/// Vytvoøí podmínku testující rovnost dvou operandù.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, DateTime value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty operandù.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		}

	}
}
