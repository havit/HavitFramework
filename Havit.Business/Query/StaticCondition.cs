using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query
{
	/// <summary>
	/// Podmínka, která je vždy vyhodnocena stejně.
	/// </summary>
	internal class StaticCondition : Condition
	{
		#region Private consts
		private const string TrueConditionText = "(0=0)";
		private const string FalseConditionText = "(0=1)";
		#endregion

		#region Private fields
		private readonly string _conditionText;
		#endregion

		#region Constructor
		private StaticCondition(string conditionText)
		{
			_conditionText = conditionText;
		}
		#endregion

		#region GetWhereStatement
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			Debug.Assert(command != null);
			Debug.Assert(whereBuilder != null);

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
		/// Vytváří instanci podmínky, která je vždy true.
		/// </summary>
		internal static Condition CreateTrue()
		{
			return new StaticCondition(TrueConditionText);
		}
		#endregion

		#region CreateFalse (static)
		/// <summary>
		/// Vytváří instanci podmínky, která je vždy false.
		/// </summary>
		internal static Condition CreateFalse()
		{
			return new StaticCondition(FalseConditionText);
		}
		#endregion
	}
}
