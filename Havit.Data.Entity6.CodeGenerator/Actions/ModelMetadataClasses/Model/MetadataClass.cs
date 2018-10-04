using System.Collections.Generic;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model
{
	public class MetadataClass
	{
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public List<MaxLengthConstant> MaxLengthConstants { get; set; }

		public class MaxLengthConstant
		{
			public string Name { get; set; }
			public int Value { get; set; }
		}
	}
}
