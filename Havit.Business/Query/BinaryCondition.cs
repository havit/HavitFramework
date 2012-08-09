using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Havit.Business.Query
{
	/// <summary>
	/// Tøída reprezentující podmínku o dvou operandech.
	/// </summary>
	[Serializable]
	public class BinaryCondition : UnaryCondition
	{
		#region Patterns
		/// <summary>
		/// Vzor pro podmínku LIKE.
		/// </summary>
		public const string LikePattern = "({0} LIKE {1})";

		/// <summary>
		/// Vzor pro podmínku rovnosti.
		/// </summary>
		public const string EqualsPattern = "({0} = {1})";

		/// <summary>
		/// Vzor pro podmínku nerovnosti.
		/// </summary>
		public const string NotEqualsPattern = "({0} <> {1})";
		#endregion

		#region Operand2
		/// <summary>
		/// Druhý operand.
		/// </summary>
		protected IOperand Operand2
		{
			get { return _operand2; }
			set { _operand2 = value; }
		}
		private IOperand _operand2; 
		#endregion

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

		#region GetWhereStatement
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE
		/// </summary>
		/// <param name="command"></param>
		/// <param name="whereBuilder"></param>
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			if (whereBuilder == null)
			{
				throw new ArgumentNullException("whereBuilder");
			}

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
			return String.Format(CultureInfo.InvariantCulture, comparisonOperatorFormatPattern, ComparisonOperatorHelper.GetOperatorText(comparisonOperator));
		}
		#endregion
	}
}
