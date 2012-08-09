using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Tøída reprezentující podmínku o tøech operandech.
	/// </summary>
	public class TernaryCondition : BinaryCondition
	{
		#region Protected fields
		/// <summary>
		/// Tøetí operand.
		/// </summary>
		protected IOperand Operand3;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci ternární podmínky.
		/// </summary>
		public TernaryCondition(string conditionPattern, IOperand operand1, IOperand operand2, IOperand operand3):
			base(conditionPattern, operand1, operand2)
		{
			if (operand3 == null)
				throw new ArgumentNullException("operand3");

			this.Operand3 = operand3;
		}
		#endregion

		#region ICondition Members
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="whereBuilder"></param>
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command), Operand2.GetCommandValue(command), Operand3.GetCommandValue(command));
		}
		#endregion
	}
}
