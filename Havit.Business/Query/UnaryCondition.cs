using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Tøída reprezentující podmínku o jednom operandu.
	/// </summary>
	public class UnaryCondition : ICondition
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
		protected IOperand Operand1;

		/// <summary>
		/// Vzor podmínky SQL dotazu.
		/// Následnì je formátován operandem (v potomcích operandy).
		/// </summary>
		protected string ConditionPattern;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci unární podmínky.
		/// </summary>
		/// <param name="conditionPattern"></param>
		/// <param name="operand"></param>
		public UnaryCondition(string conditionPattern, IOperand operand)
		{
			if (conditionPattern == null)
			{
				throw new ArgumentNullException("conditionPattern");
			}

			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}

			Operand1 = operand;
			ConditionPattern = conditionPattern;
		}
		#endregion

		#region ICondition Members
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="whereBuilder"></param>
		public virtual void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command));
		}
		#endregion
	}
}
