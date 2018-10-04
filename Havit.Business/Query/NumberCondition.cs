using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Havit.Business;
using Havit.Diagnostics.Contracts;

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
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující rovnost hodnoty.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, decimal value)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující rovnost hodnoty operandů.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			Contract.Requires<ArgumentNullException>(operand1 != null, "operand1");
			Contract.Requires<ArgumentNullException>(operand2 != null, "operand2");

			return new BinaryCondition(operand1, BinaryCondition.EqualsPattern, operand2);
		} 
		#endregion

		#region CreateIn
		/// <summary>
		/// Vytvoří podmínku testující existence hodnoty v poli integerů.
		/// </summary>
		public static Condition CreateIn(IOperand operand, int[] values)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");
			Contract.Requires<ArgumentNullException>(values != null, "values");

			if (values.Length == 0)
			{
				return StaticCondition.CreateFalse();
			}
			else if (values.Length == 1)
			{
				return NumberCondition.CreateEquals(operand, values[0]);
			}
			else
			{
				if (Math.Abs(values[0] - values[values.Length - 1]) < values.Length)
				{
					List<int> distinctValues = values.Distinct().ToList();
					if (distinctValues.Count > 1)
					{
						distinctValues.Sort();

						int firstId = distinctValues[0];
						int lastId = distinctValues[distinctValues.Count - 1];
						if ((lastId - firstId + 1) == distinctValues.Count)
						{
							return new TernaryCondition(TernaryCondition.BetweenPattern, operand, ValueOperand.Create(firstId), ValueOperand.Create(lastId));
						}
					}
					else
					{
						return NumberCondition.CreateEquals(operand, distinctValues[0]);
					}
				}
				return new InIntegersCondition(operand, values);
			}
		}

		/// <summary>
		/// Vytvoří podmínku testující existence hodnoty v poli integerů (resp. v IEnumerable&lt;int&gt;).
		/// </summary>
		public static Condition CreateIn(IOperand operand, IEnumerable<int> values)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");
			Contract.Requires<ArgumentNullException>(values != null, "values");

			return CreateIn(operand, values.ToArray());
		}
		#endregion

		#region CreateRange
		/// <summary>
		/// Vytvoří podmínku testující hodnotu v rozsahu od-do (včetně krajních hodnot).
		/// </summary>
		public static Condition CreateRange(IOperand operand, int valueFrom, int valueTo)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return CreateRange(operand, ValueOperand.Create(valueFrom), ValueOperand.Create(valueTo));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnotu v rozsahu od-do (včetně krajních hodnot).
		/// </summary>
		public static Condition CreateRange(IOperand operand, decimal valueFrom, decimal valueTo)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return CreateRange(operand, ValueOperand.Create(valueFrom), ValueOperand.Create(valueTo));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnotu v rozsahu od-do (včetně krajních hodnot).
		/// </summary>
		private static Condition CreateRange(IOperand operand, IOperand operandFrom, IOperand operandTo)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");
			Contract.Requires<ArgumentNullException>(operandFrom != null, "operandFrom");
			Contract.Requires<ArgumentNullException>(operandTo != null, "operandTo");

			return new TernaryCondition(TernaryCondition.BetweenPattern, operand, operandFrom, operandTo);
		}
		#endregion

		#region Create
		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, int value)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, decimal value)
		{
			Contract.Requires<ArgumentNullException>(operand != null, "operand");

			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
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
