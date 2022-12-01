using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model
{
	public class MetadataClass
	{
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public bool RequiresSystemNamespace => MaxLengthConstants.Any(item => item.RequiresSystemNamespace);

		public List<MaxLengthConstant> MaxLengthConstants { get; set; }

		public class MaxLengthConstant
		{
			public string Name { get; set; }
			public string Value { get; set; }
			public bool RequiresSystemNamespace { get; set; }
		}
	}
}
