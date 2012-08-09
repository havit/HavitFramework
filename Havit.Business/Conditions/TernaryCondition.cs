using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business.Conditions;

namespace Havit.MetrosPneuservis.BusinessLayer.Framework.Conditions
{
	public class TernaryCondition : BinaryCondition
	{
		IOperand Operand3;

		public TernaryCondition(string conditionPattern, IOperand operand1, IOperand operand2, IOperand operand3):
			base(conditionPattern, operand1, operand2)
		{
			if (operand3 == null)
				throw new ArgumentNullException("operand3");

			this.Operand3 = operand3;
		}

		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command), Operand2.GetCommandValue(command), Operand3.GetCommandValue(command));
		}

	}
}
