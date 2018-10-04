using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Helpers
{
	/// <summary>
	/// Pomocné metody pro práci s názvem databáze.
	/// </summary>
	public static class DatabaseNameHelper
	{
		/// <summary>
		/// Vrátí název databáze pro účely použití unit testů.
		/// Pokud existuje proměná prostředí BUILD_BUILDNUMBER, pak se použije databaseNameBase-BUILD_BUILDNUMBER, jinak jen databaseNameBase.
		/// </summary>
		public static string GetDatabaseNameForUnitTest(string databaseNameBase)
		{
			string buildNumber = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
			if (!String.IsNullOrEmpty(buildNumber))
			{
				return databaseNameBase + "-" + buildNumber;
			}
			return databaseNameBase;
		}
	}
}
