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
		public static ICondition CreateEquals(Property property, int value)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static ICondition Create(Property property, ComparisonOperator comparisonOperator, int value)
		{
			return new BinaryCondition(property, BinaryCondition.GetComparisonPattern(comparisonOperator), ValueOperand.Create(value));			
		}

	}
}
