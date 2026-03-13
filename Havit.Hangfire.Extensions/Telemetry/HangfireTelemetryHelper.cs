using System.Data.Common;

namespace Havit.Hangfire.Extensions.Telemetry;

/// <summary>
/// Helper třída pro telemetrii Hangfire.
/// </summary>
public static class HangfireTelemetryHelper
{
	/// <summary>
	/// Zachytí dotazy, které se víceméně jistě týkají Hangfire (např. přístup do jeho tabulek),
	/// ale nezachytí dotazy, které se nedotazují do dat Hangfire, např. použití databázového aplikačního zámku.
	/// Vyžaduje výchozí název databázového schématu Hangfire, tj. "HangFire".
	/// Pokud používáte jiné schéma, získejte funkci pro ověření, zda jde o dotaz týkající se Hangfire pomocí GetIsHangfireSqlCommandFunc.
	/// </summary>
	public static bool IsHangfireSqlCommand(DbCommand dbCommand)
	{
		return dbCommand?.CommandText?.Contains("[HangFire].") ?? false;
	}

	/// <summary>
	/// Zachytí dotazy, které se víceméně jistě týkají Hangfire (např. přístup do jeho tabulek),
	/// ale nezachytí dotazy, které se nedotazují do dat Hangfire, např. použití databázového aplikačního zámku.
	/// Lze zvolit název databázového schématu Hangfire, tj. "HangFire" je výchozí hodnota.
	/// Implementace je optimalizována pro opakované použití, tj. pro opakované volání s různými DbCommand objekty.
	/// </summary>
	public static Func<DbCommand, bool> GetIsHangfireSqlCommandFunc(string hangfireSchemaName = "HangFire")
	{
		string schemaNameWithBracketsAndDot = "[" + hangfireSchemaName + "].";
		return dbCommand => dbCommand?.CommandText?.Contains(schemaNameWithBracketsAndDot) ?? false;
	}
}
