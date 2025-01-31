using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query;

/// <summary>
/// Podmínka, která je vždy vyhodnocena stejně.
/// </summary>
internal class StaticCondition : Condition
{
	private const string TrueConditionText = "(0=0)";
	private const string FalseConditionText = "(0=1)";

	private readonly string _conditionText;

	private StaticCondition(string conditionText)
	{
		_conditionText = conditionText;
	}

	public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
	{
		Debug.Assert(command != null);
		Debug.Assert(whereBuilder != null);

		whereBuilder.Append(_conditionText);
	}

	public override bool IsEmptyCondition()
	{
		return false;
	}

	/// <summary>
	/// Vytváří instanci podmínky, která je vždy true.
	/// </summary>
	internal static Condition CreateTrue()
	{
		return new StaticCondition(TrueConditionText);
	}

	/// <summary>
	/// Vytváří instanci podmínky, která je vždy false.
	/// </summary>
	internal static Condition CreateFalse()
	{
		return new StaticCondition(FalseConditionText);
	}
}
