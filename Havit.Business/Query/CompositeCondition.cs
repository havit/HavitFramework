using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query;

/// <summary>
/// Předek kompozitních podmínek.
/// </summary>
public abstract class CompositeCondition : Condition
{
	private readonly string operatorBetweenOperands = null;

	/// <summary>
	/// Podmínky v kompozitu.
	/// </summary>
	public ConditionList Conditions
	{
		get { return conditions; }
	}
	private readonly ConditionList conditions;

	/// <summary>
	/// Vytvoří instanci.
	/// </summary>
	protected CompositeCondition(string operatorBetweenOperands, params Condition[] conditions)
	{
		this.conditions = new ConditionList();
		this.operatorBetweenOperands = operatorBetweenOperands;
		if (conditions != null)
		{
			for (int index = 0; index < conditions.Length; index++)
			{
				this.Conditions.Add(conditions[index]);
			}
		}
	}

	/// <summary>
	/// Vrací true, pokud jde o prázdnou podmínku a to i v případě, že jde například o zanořené AndCondition a OrCondition bez žádných "funkčních" podmínek.
	/// </summary>
	public override bool IsEmptyCondition()
	{
		foreach (Condition item in this.Conditions)
		{
			if (!item.IsEmptyCondition())
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Poskládá členské podmínky. Mezi podmínkami (operandy) je operátor zadaný v konstruktoru.
	/// </summary>
	public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
	{
		Debug.Assert(command != null);
		Debug.Assert(whereBuilder != null);

		if (this.IsEmptyCondition())
		{
			return;
		}

		//if (Conditions.Count == 1)
		//{			
		//    Conditions[0].GetWhereStatement(command, whereBuilder);
		//    return;
		//}

		whereBuilder.Append("(");
		bool renderedFirst = false;
		for (int i = 0; i < Conditions.Count; i++)
		{
			Condition condition = Conditions[i];
			if (!condition.IsEmptyCondition())
			{
				if (renderedFirst)
				{
					whereBuilder.AppendFormat(" {0} ", operatorBetweenOperands);
				}
				condition.GetWhereStatement(command, whereBuilder, sqlServerPlatform, commandBuilderOptions);
				renderedFirst = true;
			}
		}
		whereBuilder.Append(")");
	}
}
