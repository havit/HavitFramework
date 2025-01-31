using System.Data.Common;
using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query;

/// <summary>
/// Integer condition IN - třída pro vnitřní použití frameworku, řeší podmínku IN a různé implementace na různých platformách.
/// </summary>
internal class InIntegersCondition : Condition
{
	private readonly IOperand operand;
	private readonly int[] ids;

	/// <summary>
	/// Konstruktor
	/// </summary>
	internal InIntegersCondition(IOperand operand, int[] ids)
	{
		this.operand = operand;
		this.ids = ids;
	}

	/// <summary>
	/// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována (např. prázdná AndCondition).
	/// </summary>
	public override bool IsEmptyCondition()
	{
		return false;
	}

	/// <summary>
	/// Přidá část SQL příkaz pro sekci WHERE. Je VELMI doporučeno, aby byla podmínka přidána včetně závorek.
	/// </summary>
	public override void GetWhereStatement(DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
	{
		Debug.Assert(command != null);
		Debug.Assert(whereBuilder != null);

		switch (sqlServerPlatform)
		{
			case SqlServerPlatform.SqlServerCe35:
				GetWhereStatementForSqlServerCe35(command, whereBuilder);
				return;

			case SqlServerPlatform.SqlServer2005:
				throw new NotSupportedException("Režim pro SqlServerPlatform.SqlServer2005 již není podporován.");

			default:
				GetWhereStatementForSqlServer2008(command, whereBuilder, sqlServerPlatform);
				break;
		}
	}

	/// <summary>
	/// Řeší variantu podmínky where pro SQL Server CE 3.5.
	/// </summary>
	private void GetWhereStatementForSqlServerCe35(DbCommand command, StringBuilder whereBuilder)
	{
		whereBuilder.AppendFormat("({{0}} IN ({0})", String.Join(",", Array.ConvertAll<int, string>(ids, item => item.ToString())));
	}

	/// <summary>
	/// Řeší variantu podmínky where pro SQL Server 2008.
	/// </summary>
	private void GetWhereStatementForSqlServer2008(DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform)
	{
		IOperand idsOperand = IntTableOperand.Create(ids);
		whereBuilder.AppendFormat("({0} IN (SELECT [Value] FROM {1}))",
			operand.GetCommandValue(command, sqlServerPlatform),
			idsOperand.GetCommandValue(command, sqlServerPlatform));
	}
}
