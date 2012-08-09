using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;

namespace Havit.Business.Conditions
{
	public static class NumberCondition
	{
		public static ICondition CreateEquals(Property property, int value)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.FromInteger(value));
		}

		public static ICondition Create(Property property, ComparisonOperator comparisonOperator, int value)
		{
			return new BinaryCondition(property, BinaryCondition.GetComparisonPattern(comparisonOperator), ValueOperand.FromInteger(value));			
		}

	}
}
