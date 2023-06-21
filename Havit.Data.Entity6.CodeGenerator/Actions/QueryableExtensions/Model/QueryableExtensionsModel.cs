using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;

public class QueryableExtensionsModel
{
	public string NamespaceName { get; set; }
	public List<string> ModelClassesFullNames { get; set; }
}
