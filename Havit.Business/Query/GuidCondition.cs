using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující GUID hodnoty.
	/// </summary>
	public class GuidCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoøí podmínku testující rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, Guid? value)
		{
			if (value == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			return CreateEquals(operand, ValueOperand.Create(value.Value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující rovnost hodnoty operandù.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		} 
		#endregion

		#region Create
		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, Guid value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		} 
		#endregion

	}
}
