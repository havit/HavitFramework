using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Tøída reprezentující podmínku o jednom operandu.
	/// </summary>
	[Serializable]
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
		/// Následnì je formátován operandem (v potomcích operandy).
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

		#region GetWhereStatement
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE.
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

			whereBuilder.AppendFormat(ConditionPattern, Operand1.GetCommandValue(command));
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
