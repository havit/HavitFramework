using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers.NamingConventions
{
	public static class NamespaceHelper
	{
		public static string GetDefaultNamespace(string projectName)
		{
			return Business.BusinessLayerGenerator.Helpers.NamingConventions.NamespaceHelper.GetDefaultNamespace().Replace("BusinessLayer", projectName);
		}

		public static string GetNamespaceName(Table table, string project, bool withDefaultNamespace = true)
		{
			return Business.BusinessLayerGenerator.Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, withDefaultNamespace).Replace("BusinessLayer", project);
		}
	}
}
