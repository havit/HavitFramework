using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers.NamingConventions
{
	public static class ClassHelper
	{
		public static string GetClassFullName(Table table, string projectName, bool withDefaultNamespace = true)
		{
			return Business.BusinessLayerGenerator.Helpers.NamingConventions.ClassHelper.GetClassFullName(table, withDefaultNamespace).Replace("BusinessLayer", projectName);
		}
    }
}
