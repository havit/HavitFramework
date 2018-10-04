using Havit.Business.BusinessLayerGenerator.Helpers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers
{
    public static class StoredProcedureHelper
	{
		public static string GetStringExtendedProperty(this StoredProcedure storedProcedure, string key)
		{
			return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(storedProcedure), key);
		}
	}
}