using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Model
{
	public class QueryableExtensionsModel
	{
		public string NamespaceName { get; set; }
		public List<string> ModelClassesFullNames { get; set; }
	}
}
