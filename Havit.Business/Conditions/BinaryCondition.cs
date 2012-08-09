using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	/// <summary>
	/// Tøída reprezentující podmínku o dvou operandech.
	/// </summary>
	public class BinaryCondition : UnaryCondition, ICondition
	{
		#region Patterns
		/// <summary>
		/// Vzor pro podmínku LIKE
		/// </summary>
		public const string LikePattern = "({0} LIKE {1})";

		/// <summary>
		/// Vzor pro podmínku rovnosti.
		/// </summary>
		public const string EqualsPattern = "({0} = {1})";
		#endregion

		/// <summary>
		/// Druhý operand.
		/// </summary>
		protected IOperand Operand2;

		#region Constructors
		/// <summary>
		/// Vytvoøí binární (dvojoperandovou) podmínku.
		/// </summary>
		public BinaryCondition(string conditionPattern, IOperand operand1, IOperand operand2) 
			: base(conditionPattern, operand1)
		{
			if (operand2 == null)
				throw new ArgumentNullException("operand2");

			Operand2 = operand2;
		}

		/// <summary>
		/// Vytvoøí binární (dvojoperandovou) podmínku.
		/// </summary>
		public BinaryCondition(IOperand operand1, string conditionPattern, IOperand operand2)
			: this(conditionPattern, operand1, operand2)
		{
		}
		#endregion

		#region ICondition Members

		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command), Operand2.GetCommandValue(command));
		}

		#endregion

		#region GetComparisonPattern
		/// <summary>
		/// Vrátí vzor podmínky pro bìžné porovnání dvou hodnot (vrací napø. "({0} = {1})").
		/// </summary>
		public static string GetComparisonPattern(ComparisonOperator comparisonOperator)
		{
			const string comparisonOperatorFormatPattern = "({{0}} {0} {{1}})";
			return String.Format(comparisonOperatorFormatPattern, ComparisonOperatorHelper.GetOperatorText(comparisonOperator));
		}
		#endregion
	}
}
