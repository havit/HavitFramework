using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	public class NullCondition
	{
		public static ICondition CreateIsNull(IOperand operand)
		{
			return new UnaryCondition(UnaryCondition.IsNullPattern, operand);
		}

		public static ICondition CreateIsNotNull(IOperand operand)
		{
			return new UnaryCondition(UnaryCondition.IsNotNullPattern, operand);
		}
	}
}
