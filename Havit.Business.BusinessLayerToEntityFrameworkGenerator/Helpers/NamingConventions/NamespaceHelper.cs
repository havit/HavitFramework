using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
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
			string namespaceName = Business.BusinessLayerGenerator.Helpers.NamingConventions.NamespaceHelper.GetNamespaceName(table, withDefaultNamespace).Replace("BusinessLayer", project);

			// override for M-N tables: they likely won't have extended property for namespace, so we fallback to namespace of the first table
			if (TableHelper.IsJoinTable(table))
			{
				var defaultNamespace = withDefaultNamespace ? BusinessLayerGenerator.Helpers.NamingConventions.NamespaceHelper.GetDefaultNamespace().Replace("BusinessLayer", project) : "";
				if (defaultNamespace == namespaceName)
				{
					var split = table.Name.Split('_');
					string firstTableName = null;
					if (split.Length > 1)
					{
						// Prefer first table from join table name (instead of whatever order FKs are in)
						firstTableName = table.ForeignKeys.Cast<ForeignKey>().FirstOrDefault(fk => fk.ReferencedTable == split[0])?.ReferencedTable;
					}

					firstTableName = firstTableName ?? table.ForeignKeys.Cast<ForeignKey>().FirstOrDefault().ReferencedTable;

					Table firstTable = table.Parent.Tables[firstTableName];
					return GetNamespaceName(firstTable, project, withDefaultNamespace);
				}
			}

			return namespaceName ?? string.Empty;
		}
	}
}
