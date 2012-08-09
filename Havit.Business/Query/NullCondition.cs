using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující null hodnoty.
	/// </summary>
	public static class NullCondition
	{
		/// <summary>
		/// Vytvoøí podmínku testující hodnotu na NULL.
		/// </summary>
		public static Condition CreateIsNull(IOperand operand)
		{
			return new UnaryCondition(UnaryCondition.IsNullPattern, operand);
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnotu na NOT NULL.
		/// </summary>
		public static Condition CreateIsNotNull(IOperand operand)
		{
			return new UnaryCondition(UnaryCondition.IsNotNullPattern, operand);
		}
	}
}
