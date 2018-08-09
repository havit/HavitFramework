using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Helpers
{
	public static class DatabaseNameHelper
	{
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
