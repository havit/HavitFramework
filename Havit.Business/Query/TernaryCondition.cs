using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Třída reprezentující podmínku o třech operandech.
	/// </summary>
	public class TernaryCondition : BinaryCondition
	{
		#region Patterns
		/// <summary>
		/// Vzor pro podmínku LIKE.
		/// </summary>
		internal const string BetweenPattern = "({0} BETWEEN {1} AND {2})";
		#endregion

		#region Protected fields
		/// <summary>
		/// Třetí operand.
		/// </summary>
		public IOperand Operand3
		{
			get { return _operand3; }
			set { _operand3 = value; }
		}
		private IOperand _operand3;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci ternární podmínky.
		/// </summary>
		public TernaryCondition(string conditionPattern, IOperand operand1, IOperand operand2, IOperand operand3) : base(conditionPattern, operand1, operand2)
		{
			Contract.Requires<ArgumentNullException>(operand3 != null, "operand3");

			this.Operand3 = operand3;
		}
		#endregion

		#region GetWhereStatement
		/// <summary>
		/// Přidá část SQL příkaz pro sekci WHERE.
		/// </summary>
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			Debug.Assert(command != null);
			Debug.Assert(whereBuilder != null);

			whereBuilder.AppendFormat(ConditionPattern,
				Operand1.GetCommandValue(command, sqlServerPlatform),
				Operand2.GetCommandValue(command, sqlServerPlatform),
				Operand3.GetCommandValue(command, sqlServerPlatform));
		}
		#endregion
	}
}
