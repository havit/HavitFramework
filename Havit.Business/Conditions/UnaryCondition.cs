using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	public class UnaryCondition: ICondition
	{
		public const string IsNullPattern = "({0} IS NULL)";
		public const string IsNotNullPattern = "({0} IS NOT NULL)";

		protected IOperand Operand1;
		protected string ConditionPattern;

		public UnaryCondition(string conditionPattern, IOperand operand)
		{
			if (conditionPattern == null)
				throw new ArgumentNullException("conditionPattern");

			if (operand == null)
				throw new ArgumentNullException("operand");

			Operand1 = operand;
			ConditionPattern = conditionPattern;
		}

		#region ICondition Members

		public virtual void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command));
		}

		#endregion
	}
}
