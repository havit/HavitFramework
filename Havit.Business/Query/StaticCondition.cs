using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Podmínka, která je vždy vyhodnocena stejnì.
	/// </summary>
	internal class StaticCondition: Condition
	{
		private const string TrueConditionText = "(0=0)";
		private const string FalseConditionText = "(0=1)";

		private string _conditionText;

		#region Constructor
		private StaticCondition(string conditionText)
		{
			_conditionText = conditionText;
		}
		#endregion

		#region GetWhereStatement
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

			whereBuilder.Append(_conditionText);
		}
		#endregion

		#region IsEmptyCondition
		public override bool IsEmptyCondition()
		{
			return false;
		} 
		#endregion

		#region CreateTrue (static)
		/// <summary>
		/// Vytváøí instanci podmínky, která je vždy true.
		/// </summary>
		internal static Condition CreateTrue()
		{
			return new StaticCondition(TrueConditionText);
		}
		#endregion

		#region CreateFalse (static)
		/// <summary>
		/// Vytváøí instanci podmínky, která je vždy false.
		/// </summary>
		internal static Condition CreateFalse()
		{
			return new StaticCondition(FalseConditionText);
		}
		#endregion
	}
}
