namespace Havit.Data.Entity.Helpers;

/// <summary>
/// Pomocné metody pro práci s názvem databáze.
/// </summary>
public static class DatabaseNameHelper
{
	/// <summary>
	/// Vrátí název databáze pro účely použití unit testů.
	/// Pokud existuje proměná prostředí AGENT_NAME, pak se použije databaseNameBase-AGENT_NAME, jinak jen databaseNameBase.
	/// </summary>
	public static string GetDatabaseNameForUnitTest(string databaseNameBase)
	{
		string agentName = Environment.GetEnvironmentVariable("AGENT_NAME");
		if (!String.IsNullOrEmpty(agentName))
		{
			return databaseNameBase + "-" + agentName;
		}
		return databaseNameBase;
	}
}
