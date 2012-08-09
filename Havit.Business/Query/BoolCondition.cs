using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínku testující logickou hodnotu.
	/// </summary>	
	public static class BoolCondition
	{
		/// <summary>
		/// Vytvoøí podmínku pro vlastnost rovnou dané hodnotì.
		/// </summary>
		public static Condition CreateEquals(PropertyInfo property, bool? value)
		{
			if (value == null)
			{
				return NullCondition.CreateIsNull(property);
			}
			else
			{
				return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.Create(value.Value));
			}
		}

		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu true.
		/// </summary>
		public static Condition CreateTrue(PropertyInfo property)
		{
			return CreateEquals(property, true);
		}

		/// <summary>
		/// Vytvoøí podmínku testující vlastnost na hodnotu false.
		/// </summary>
		public static Condition CreateFalse(PropertyInfo property)
		{
			return CreateEquals(property, false);
		}
	}
}
