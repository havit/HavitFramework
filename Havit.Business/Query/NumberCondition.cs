using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky testující číselné hodnoty.
	/// </summary>
	public static class NumberCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoří podmínku testující rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, int value)
		{
			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, decimal value)
		{
			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující rovnost hodnoty operandů.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		} 
		#endregion

		#region Create
		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, int value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, decimal value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		} 
		#endregion
	}
}
