using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.BusinessLayerToEntityFrameworkGenerator.Helpers.NamingConventions
{
	public static class ClassHelper
	{
		public static string GetClassFullName(Table table, string projectName, bool withDefaultNamespace = true)
		{
			return Business.BusinessLayerGenerator.Helpers.NamingConventions.ClassHelper.GetClassFullName(table, withDefaultNamespace).Replace("BusinessLayer", projectName);
		}
    }
}
