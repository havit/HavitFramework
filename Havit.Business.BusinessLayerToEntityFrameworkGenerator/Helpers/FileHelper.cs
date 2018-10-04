using System;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers
{
	public static class FileHelper
	{
		public static string GetFilename(Table table, string projectName, string suffix, string organizationFolder)
		{
			string namespaceName = NamingConventions.NamespaceHelper.GetNamespaceName(table, projectName, false);
			string className = ClassHelper.GetClassName(table);
			return Path.Combine(projectName, BusinessLayerGenerator.Helpers.FileHelper.GetFilename(namespaceName, className, suffix, organizationFolder));
		}
	}
}