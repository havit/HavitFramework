using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Havit.Data.SqlServer;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Třída reprezentující podmínku o dvou operandech.
	/// </summary>
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
		/// Vytvoří binární (dvojoperandovou) podmínku.
		/// </summary>
		public BinaryCondition(string conditionPattern, IOperand operand1, IOperand operand2) 
			: base(conditionPattern, operand1)
		{
			Contract.Requires<ArgumentNullException>(operand2 != null, "operand2");

			Operand2 = operand2;
		}

		/// <summary>
		/// Vytvoří binární (dvojoperandovou) podmínku.
		/// </summary>
		public BinaryCondition(IOperand operand1, string conditionPattern, IOperand operand2)
			: this(conditionPattern, operand1, operand2)
		{
		}
		#endregion

		#region GetWhereStatement
		/// <summary>
		/// Přidá část SQL příkaz pro sekci WHERE
		/// </summary>
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			Debug.Assert(command != null);
			Debug.Assert(whereBuilder != null);

			whereBuilder.AppendFormat(ConditionPattern,
				Operand1.GetCommandValue(command, sqlServerPlatform),
				Operand2.GetCommandValue(command, sqlServerPlatform));
		}
		#endregion

		#region GetComparisonPattern
		/// <summary>
		/// Vrátí vzor podmínky pro běžné porovnání dvou hodnot (vrací např. "({0} = {1})").
		/// </summary>
		internal static string GetComparisonPattern(ComparisonOperator comparisonOperator)
		{
			const string comparisonOperatorFormatPattern = "({{0}} {0} {{1}})";
			return String.Format(CultureInfo.InvariantCulture, comparisonOperatorFormatPattern, ComparisonOperatorHelper.GetOperatorText(comparisonOperator));
		}
		#endregion
	}
}
