using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Třída reprezentující podmínku o jednom operandu.
	/// </summary>
	public class UnaryCondition : Condition
	{
		#region Patterns
		/// <summary>
		/// Vzor pro podmínku IS NULL.
		/// </summary>
		public const string IsNullPattern = "({0} IS NULL)";

		/// <summary>
		/// Vzor pro podmínku IS NOT NULL.
		/// </summary>
		public const string IsNotNullPattern = "({0} IS NOT NULL)";
		#endregion

		#region Protected fields
		/// <summary>
		/// Operand.
		/// </summary>
		public IOperand Operand1
		{
			get { return _operand1; }
			set { _operand1 = value; }
		}
		private IOperand _operand1;

		/// <summary>
		/// Vzor podmínky SQL dotazu.
		/// Následně je formátován operandem (v potomcích operandy).
		/// </summary>
		public string ConditionPattern
		{
			get { return _conditionPattern; }
			set { _conditionPattern = value; }
		}
		private string _conditionPattern;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci unární podmínky.
		/// </summary>
		public UnaryCondition(string conditionPattern, IOperand operand)
		{
			Contract.Requires<ArgumentNullException>(conditionPattern != null, nameof(conditionPattern));
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			Operand1 = operand;
			ConditionPattern = conditionPattern;
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

			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command, sqlServerPlatform));
		}
		
		#endregion

		#region IsEmptyCondition
		/// <summary>
		/// Udává, zda je podmínka prázdná.
		/// Vrací vždy false.
		/// </summary>
		public override bool IsEmptyCondition()
		{
			return false;
		}
		#endregion
	}
}
