using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business.Conditions;

namespace Havit.Business.Conditions
{
	/// <summary>
	/// Vytváøí podmínku testující logickou hodnotu.
	/// </summary>	
	public static class BoolCondition
	{
		/// <summary>
		/// Vytvoøí podmínku pro vlastnost rovnou dané hodnotì.
		/// </summary>
		public static ICondition CreateEquals(Property property, bool? value)
		{
			if (value == null)
				return NullCondition.CreateIsNull(property);
			else
				return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.FromBoolean(value.Value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu true.
		/// </summary>
		public static ICondition CreateTrue(Property property)
		{
			return CreateEquals(property, true);
		}

		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu false.
		/// </summary>
		public static ICondition CreateFalse(Property property)
		{
			return CreateEquals(property, false);
		}
	}
}
