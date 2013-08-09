using System;
using System.Collections.Generic;
using System.Text;

using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky testující datumy.
	/// </summary>
	public static class DateCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoří podmínku testující rovnost datumů. Jeli datum roven null, testuje se na IS NULL.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, DateTime? dateTime)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			if (dateTime == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			else
			{
				return CreateEquals(operand, dateTime.Value);
			}
		} 

		/// <summary>
		/// Vytvoří podmínku testující rovnost datumů.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, DateTime dateTime)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return CreateEquals(operand, ValueOperand.Create(dateTime));
		} 

		/// <summary>
		/// Vytvoří podmínku testující rovnost dvou operandů.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			Contract.Requires<ArgumentNullException>(operand1 != null, "operand1");
			Contract.Requires<ArgumentNullException>(operand2 != null, "operand2");

			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		} 
		#endregion

		#region Create
		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, DateTime value)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		} 

		/// <summary>
		/// Vytvoří podmínku testující hodnoty operandů.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			Contract.Requires<ArgumentNullException>(operand1 != null, "operand1");
			Contract.Requires<ArgumentNullException>(operand2 != null, "operand2");

			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		}
		#endregion

	}
}
